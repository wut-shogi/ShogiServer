echo "Building engine library"
cd ./shogi_engine

echo "Preparing build directory"
if (Test-Path ./build) {
    echo "Creating build directory"
    mkdir build | Out-Null
}
cd ./build

echo "Executing build task"
cmake -DSHOGI_CUDA_SUPPORT=OFF ..
cmake --build .

echo "Copying output library"
cp ./core/Debug/shogi_engine.dll ../../shogi_engine.dll
