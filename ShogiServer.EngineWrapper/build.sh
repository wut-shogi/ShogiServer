#!/bin/sh

echo "Enter engine directory"
cd shogi_engine

echo "Clear build directory"
rm -rf build
mkdir build
cd build

echo "Build with cmake and make"
cmake -DSHOGI_CUDA_SUPPORT=OFF .. >/dev/null
make shogi_engine 2>/dev/null

echo "Copy engine library"
cp ./core/libshogi_engine.so ../../shogi_engine.so
