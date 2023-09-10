let cmd = mc.newCommand(
    'export',
    'Export blocks in a region to a JSON file',
    PermType.Console
)

cmd.mandatory(
    'start',
    ParamType.BlockPos
)

cmd.mandatory(
    'end',
    ParamType.BlockPos
)

cmd.overload(['start', 'end'])

cmd.setCallback((cmd, origin, output, results) => {
    let startPos = results['start']
    let endPos = results['end']

    if (startPos == undefined) {
        logger.error('Start position is null')
        return
    }

    if (endPos == undefined) {
        logger.error('End position is null')
        return
    }

    if (startPos.dim != endPos.dim) {
        logger.error('Start and end positions must be in the same dimension')
        return
    }

    if (startPos.x > endPos.x || startPos.y > endPos.y || startPos.z > endPos.z) {
        logger.error('Start position must be less than end position')
        return
    }

    logger.log('Exporting level data...')
    let startTimestamp = Date.now()

    // Get structure
    let structureNbt = mc.getStructure(startPos, endPos)

    // Get JSON string
    let jsonStr = structureNbt.toString()

    // Write to file
    File.mkdir('plugins/level_exporter')
    if (!File.writeTo('plugins/level_exporter/level_data.json', jsonStr)) {
        logger.error('Failed to write to plugins/level_exporter/level_data.json')
        return
    }

    logger.log('Successfully exported level data to plugins/level_exporter/level_data.json')
    logger.log(`Took ${Date.now() - startTimestamp}ms`)
})

cmd.setup()