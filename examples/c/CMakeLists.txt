cmake_minimum_required(VERSION 3.18)

project(agent)


# Build dependencies
add_subdirectory(sdk)


# Build agent
aux_source_directory(src SRC_LIST)
add_executable(${PROJECT_NAME} ${SRC_LIST})

if(MSVC)
    target_compile_options(${PROJECT_NAME} PUBLIC /W4)
else()
    target_compile_options(${PROJECT_NAME} PUBLIC -Wall -Wextra -pedantic)
endif()

target_link_libraries(${PROJECT_NAME} ncsdk)
