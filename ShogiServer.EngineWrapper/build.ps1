echo "Building engine library"
cd ./shogi_engine

echo "Preparing build directory"
if (Test-Path ./build) {
    echo "Clearing build directory"
    rmdir ./build
}
mkdir build | Out-Null
cd ./build

echo "Executing build task"
cmake ..