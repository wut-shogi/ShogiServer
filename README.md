# Instrukcja instalacji
## Wymagane narzędzia
- .NET 7 i ASP.NET CORE 7
- Nvidia CUDA SDK
- CMake (3.22^) (Uwaga: na systemie operacyjnym Windows CMake z Visual Studio nie jest rozpoznawany. Należy zainstalować CMake bezpośrednio ze strony projektu)

## Instrukcja kompilacji
```
git clone --recurse-submodules https://github.com/wut-shogi/ShogiServer.git
dotnet build
```

## Instrukcja uruchomienia
```
dotnet run
```
