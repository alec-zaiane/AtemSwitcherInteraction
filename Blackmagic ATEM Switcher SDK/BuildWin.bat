if "%SwitchersSDK_Include_DIR%"=="" (set SwitchersSDK_Include_DIR=%cd%\Blackmagic ATEM Switchers SDK 8.1\Windows\include)

set "VSCMD_START_DIR=%cd%"
call "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\VC\Auxiliary\Build\vcvars64.bat"

mkdir build
cd build
cmake -G "Visual Studio 15 2017" -A x64 -DSWITCHERS_SDK_INCLUDE_DIR="%SwitchersSDK_Include_DIR%" ..
cmake --build . --config Release
