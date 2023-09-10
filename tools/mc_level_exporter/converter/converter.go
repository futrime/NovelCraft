package main

import (
	"archive/zip"
	"compress/flate"
	"encoding/json"
	"fmt"
	"io"
	"os"
	"strings"

	"github.com/novelcraft/minecraftlevelexporter/logger"
	"github.com/xeipuuv/gojsonschema"
)

// Section represents a section of a Minecraft world. It contains the x, y, z
// coordinates of the section and a 3D array of blocks.
type Section struct {
	X      int   `json:"x"`
	Y      int   `json:"y"`
	Z      int   `json:"z"`
	Blocks []int `json:"blocks"`
}

type Size struct {
	X int
	Y int
	Z int
}

// inputJsonSchema is the json schema for the input json file. It must be a 3D
// array of integers.
const inputJsonSchema string = `
{
	"$schema": "http://json-schema.org/draft-07/schema#",
	"type": "object",
	"properties": {
		"size": {
			"type": "array",
			"items": {
				"type": "integer",
				"minimum": 1
			},
			"minItems": 3,
			"maxItems": 3
		},
		"structure": {
			"type": "object",
			"properties": {
				"block_indices": {
					"type": "array",
					"items": {
						"type": "array",
						"items": {
							"type": "integer"
						},
						"minItems": 1
					},
					"minItems": 1
				},
				"palette": {
					"type": "object",
					"properties": {
						"default": {
							"type": "object",
							"properties": {
								"block_palette": {
									"type": "array",
									"items": {
										"type": "object",
										"properties": {
											"name": {
												"type": "string"
											}
										},
										"required": ["name"]
									},
									"minItems": 1
								}
							},
							"required": ["block_palette"]
						}
					},
					"required": ["default"]
				}
			},
			"required": ["block_indices", "palette"]
		}
	},
	"required": ["size", "structure"]
}
`

const blockDictJsonSchema string = `
{
	"$schema": "http://json-schema.org/draft-07/schema#",
	"type": "object",
	"patternProperties": {
		"^minecraft:\\w+$": {
			"type": "integer"
		}
	}
}
`

const DefaultBlockId int = 0
const OutOfRangeBlockId int = -1

func main() {
	// Validate arguments.
	if len(os.Args) < 4 {
		logger.Error("too few arguments")
		logger.Info("Usage: %s <input file> <dictionary> <output file>", os.Args[0])
		return
	}

	if len(os.Args) > 4 {
		logger.Error("too many arguments")
		logger.Info("Usage: %s <input file> <dictionary> <output file>", os.Args[0])
		return
	}

	if !strings.HasSuffix(os.Args[1], ".json") || !strings.HasSuffix(os.Args[2], ".json") {
		logger.Error("input file and dictionary file must be JSON files")
		return
	}

	if !strings.HasSuffix(os.Args[3], ".nclevel") {
		logger.Error("output file must be a .nclevel file")
		return
	}

	// Read input file.
	jsonContent, err := os.ReadFile(os.Args[1])
	if err != nil {
		logger.Error("failed to read input file: %s", err.Error())
		return
	}

	// Validate input file.
	schemaLoader := gojsonschema.NewStringLoader(inputJsonSchema)
	documentLoader := gojsonschema.NewBytesLoader((jsonContent))

	result, err := gojsonschema.Validate(schemaLoader, documentLoader)
	if err != nil {
		logger.Error("failed to validate input file: %s", err.Error())
		return
	}

	if !result.Valid() {
		logger.Error("input file is not valid")
		return
	}

	// Read input JSON to a map.
	var inputMap map[string]interface{}
	err = json.Unmarshal(jsonContent, &inputMap)
	if err != nil {
		logger.Error("failed to parse input file: %s", err.Error())
		return
	}

	// Read block dictionary.
	jsonContent, err = os.ReadFile(os.Args[2])
	if err != nil {
		logger.Error("failed to read block dictionary: %s", err.Error())
		return
	}

	// Validate block dictionary.
	schemaLoader = gojsonschema.NewStringLoader(blockDictJsonSchema)
	documentLoader = gojsonschema.NewBytesLoader((jsonContent))

	result, err = gojsonschema.Validate(schemaLoader, documentLoader)
	if err != nil {
		logger.Error("failed to validate block dictionary: %s", err.Error())
		return
	}

	if !result.Valid() {
		logger.Error("block dictionary is not valid")
		return
	}

	// Read block dictionary to a map.
	var blockDict map[string]int
	err = json.Unmarshal(jsonContent, &blockDict)
	if err != nil {
		logger.Error("failed to parse block dictionary: %s", err.Error())
		return
	}

	// Validate the input map.
	if !validateInput(inputMap) {
		logger.Error("input map is not valid")
		return
	}

	// Convert the input map to an array of sections.
	sections := convertToSections(inputMap, blockDict, DefaultBlockId, OutOfRangeBlockId)

	// Make the level data map
	levelData := make(map[string]interface{})
	levelData["type"] = "level_data"
	levelData["sections"] = sections
	levelData["entities"] = make([]interface{}, 0)
	levelData["players"] = make([]interface{}, 0)

	// Convert the level data map to json.
	levelDataJson, err := json.Marshal(levelData)
	if err != nil {
		logger.Error("failed to convert level data to json: %s", err.Error())
		return
	}

	// Write the level data to a file.
	err = writeLevelDataFile(levelDataJson, os.Args[3])
	if err != nil {
		logger.Error("failed to write level data to file: %s", err.Error())
		return
	}

	logger.Info("Successfully converted input file to level data")
}

// validateInput checks if the input map is a 3D cubic array of integers.
func validateInput(inputMap map[string]interface{}) bool {
	expectedBlockCount := int(inputMap["size"].([]interface{})[0].(float64) * inputMap["size"].([]interface{})[1].(float64) *
		inputMap["size"].([]interface{})[2].(float64))

	blocks := inputMap["structure"].(map[string]interface{})["block_indices"].([]interface{})[0].([]interface{})

	blockCount := len(blocks)

	// Check if the number of blocks is correct.
	if blockCount != expectedBlockCount {
		return false
	}

	// Get the number of different block types.
	var expectedBlockTypeCount int = 0
	for _, e := range blocks {
		num := int(e.(float64))
		if num > expectedBlockTypeCount {
			expectedBlockTypeCount = num
		}
	}
	expectedBlockTypeCount++ // Add 1 because the block count starts at 0.

	blockTypes := inputMap["structure"].(map[string]interface{})["palette"].(map[string]interface{})["default"].(map[string]interface{})["block_palette"].([]interface{})
	blockTypeCount := len(blockTypes)

	// Check if the number of block types is correct.
	if blockTypeCount != expectedBlockTypeCount {
		return false
	} else {
		return true
	}
}

// convertToSections converts the input map to an array of sections.
func convertToSections(inputMap map[string]interface{}, blockDict map[string]int, defaultBlockId int, outOfRangeBlockId int) []Section {
	exportedSize := Size{
		X: int(inputMap["size"].([]interface{})[0].(float64)),
		Y: int(inputMap["size"].([]interface{})[1].(float64)),
		Z: int(inputMap["size"].([]interface{})[2].(float64)),
	}

	rawBlocks := inputMap["structure"].(map[string]interface{})["block_indices"].([]interface{})[0].([]interface{})

	blockIndiceDict := make([]string, 0)
	blockTypes := inputMap["structure"].(map[string]interface{})["palette"].(map[string]interface{})["default"].(map[string]interface{})["block_palette"].([]interface{})
	for _, blockType := range blockTypes {
		blockIndiceDict = append(blockIndiceDict, blockType.(map[string]interface{})["name"].(string))
	}

	// Transform the block indices to block ids.
	blocks := make([]int, len(rawBlocks))
	for i, blockIndice := range rawBlocks {
		blockName := blockIndiceDict[int(blockIndice.(float64))]
		blockId, ok := blockDict[blockName]
		if !ok {
			blockId = defaultBlockId
		}
		blocks[i] = blockId
	}

	// Create the sections.
	sectionCount := Size{
		X: (exportedSize.X + 15) / 16,
		Y: (exportedSize.Y + 15) / 16,
		Z: (exportedSize.Z + 15) / 16,
	}

	sections := make([]Section, 0)
	for x := 0; x < sectionCount.X; x++ {
		for y := 0; y < sectionCount.Y; y++ {
			for z := 0; z < sectionCount.Z; z++ {
				sectionBlocks := make([]int, 4096)

				for i := 0; i < 16; i++ {
					for j := 0; j < 16; j++ {
						for k := 0; k < 16; k++ {
							if x*16+i >= exportedSize.X || y*16+j >= exportedSize.Y || z*16+k >= exportedSize.Z {
								sectionBlocks[i*256+j*16+k] = outOfRangeBlockId
							} else {
								sectionBlocks[i*256+j*16+k] = blocks[getOffset(x*16+i, y*16+j, z*16+k, exportedSize)]
							}
						}
					}
				}

				section := Section{
					X:      x * 16,
					Y:      y * 16,
					Z:      z * 16,
					Blocks: sectionBlocks,
				}

				sections = append(sections, section)
			}
		}
	}

	return sections
}

func writeLevelDataFile(levelDataJson []byte, filePath string) error {
	// Remove level.dat if it exists.
	err := os.Remove(filePath)
	if err != nil && !os.IsNotExist(err) {
		return fmt.Errorf("failed to remove %s: %s", filePath, err.Error())
	}

	// Create export target file.
	zipFile, err := os.Create(filePath)
	if err != nil {
		return fmt.Errorf("failed to create %s: %s", filePath, err.Error())
	}
	defer zipFile.Close()

	zipWriter := zip.NewWriter(zipFile)
	defer zipWriter.Close()

	zipWriter.RegisterCompressor(zip.Deflate, func(out io.Writer) (io.WriteCloser, error) {
		return flate.NewWriter(out, flate.BestCompression)
	})

	// Create level.dat in the zip file.
	levelDatWriter, err := zipWriter.Create("level.dat")
	if err != nil {
		return fmt.Errorf("failed to create level.dat in %s: %s", filePath, err.Error())
	}

	levelDatZipWriter := zip.NewWriter(levelDatWriter)
	defer levelDatZipWriter.Close()

	levelDatZipWriter.RegisterCompressor(zip.Deflate, func(out io.Writer) (io.WriteCloser, error) {
		return flate.NewWriter(out, flate.BestCompression)
	})

	// Create level_data.json in level.dat.
	levelDataJsonWriter, err := levelDatZipWriter.Create("level_data.json")
	if err != nil {
		return fmt.Errorf("failed to create level_data.json in %s: %s", filePath, err.Error())
	}

	_, err = levelDataJsonWriter.Write(levelDataJson)
	if err != nil {
		return fmt.Errorf("failed to write level_data.json to %s: %s", filePath, err.Error())
	}

	return nil
}

func getOffset(x int, y int, z int, size Size) int {
	return x*size.Y*size.Z + y*size.Z + z
}
