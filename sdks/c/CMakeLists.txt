cmake_minimum_required(VERSION 3.18)

# NovelCraft SDK.
project(ncsdk)


################################################################
# Options. You can change these options to control the build.
################################################################
option(BUILD_SAMPLES "Build samples." ON)
option(BUILD_TEST "Build test." OFF)
################################################################


# Build dependencies.
set(ARGPARSE_SHARED OFF CACHE BOOL "" FORCE)
add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/third_party/argparse)
unset(ARGPARSE_SHARED)

add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/third_party/jansson)

set(DISABLE_WERROR ON CACHE BOOL "" FORCE)
set(LWS_WITH_SHARED OFF CACHE BOOL "" FORCE)
set(LWS_WITH_SSL OFF CACHE BOOL "" FORCE)
add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/third_party/libwebsockets)
unset(DISABLE_WERROR)
unset(LWS_WITH_SHARED)
unset(LWS_WITH_SSL)

# Suppress warnings from dependencies.
if (MSVC)
    target_compile_options(argparse PRIVATE /w)
    target_compile_options(jansson PRIVATE /w)
    target_compile_options(websockets PRIVATE /w)
else()
    target_compile_options(argparse PRIVATE -w)
    target_compile_options(jansson PRIVATE -w)
    target_compile_options(websockets PRIVATE -w)
endif()


# Build NovelCraft SDK.
aux_source_directory(src SRC_LIST)
add_library(${PROJECT_NAME} STATIC ${SRC_LIST})
unset(SRC_LIST)

target_include_directories(${PROJECT_NAME} PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}/include)

if(CMAKE_BUILD_TYPE STREQUAL "Debug")
    target_compile_definitions(${PROJECT_NAME} PUBLIC _DEBUG)
endif()
if(MSVC)
    target_compile_definitions(${PROJECT_NAME} PUBLIC _CRT_SECURE_NO_WARNINGS)
    target_compile_options(${PROJECT_NAME} PUBLIC /W4)
else()
    target_compile_options(${PROJECT_NAME} PUBLIC -Wall -Wextra -pedantic)
endif()
set_property(TARGET ${PROJECT_NAME} PROPERTY C_STANDARD 11)

target_link_libraries(${PROJECT_NAME} PUBLIC argparse jansson websockets)


# Build samples.
if(BUILD_SAMPLES)
    # Define a function to build samples.
    function(build_sample SAMPLE_NAME)
        aux_source_directory(samples/${SAMPLE_NAME} SRC_LIST)
        add_executable(${SAMPLE_NAME} ${SRC_LIST})
        unset(SRC_LIST)
        set_property(TARGET ${SAMPLE_NAME} PROPERTY C_STANDARD 11)
        if(MSVC)
            target_compile_definitions(${SAMPLE_NAME} PUBLIC _CRT_SECURE_NO_WARNINGS)
            target_compile_options(${SAMPLE_NAME} PUBLIC /W4)
        else()
            target_compile_options(${SAMPLE_NAME} PUBLIC -Wall -Wextra -pedantic)
        endif()

        target_link_libraries(${SAMPLE_NAME} ${PROJECT_NAME})
    endfunction()

    # Build samples.
    build_sample(log_messages)
    build_sample(run_client)
endif()


# Build test.
if(BUILD_TEST)
    # Configure GoogleTest.
    include(FetchContent)

    FetchContent_Declare(
        googletest
        URL https://github.com/google/googletest/archive/03597a01ee50ed33e9dfd640b249b4be3799d395.zip
        DOWNLOAD_EXTRACT_TIMESTAMP True
    )
    # For Windows: Prevent overriding the parent project's compiler/linker settings
    set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
    FetchContent_MakeAvailable(googletest)

    enable_testing()


    # Build test.
    aux_source_directory(test SRC_LIST)
    add_executable(${PROJECT_NAME}_test ${SRC_LIST})
    unset(SRC_LIST)
    if(MSVC)
        target_compile_definitions(${PROJECT_NAME}_test PUBLIC _CRT_SECURE_NO_WARNINGS)
        target_compile_options(${PROJECT_NAME}_test PUBLIC /W4)
    else()
        target_compile_options(${PROJECT_NAME}_test PUBLIC -Wall -Wextra -pedantic)
    endif()

    target_link_libraries(${PROJECT_NAME}_test ${PROJECT_NAME} GTest::gtest_main)

    include(GoogleTest)
    gtest_discover_tests(${PROJECT_NAME}_test)
endif()
