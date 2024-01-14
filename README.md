# Instrukcja instalacji
## Wymagane narzędzia
- Git (oraz dostęp do Internetu)
- .NET 7 SDK i ASP.NET CORE 7 SDK
- Nvidia CUDA SDK
- CMake (3.22^) (Uwaga: na systemie operacyjnym Windows CMake z Visual Studio może nie być domyślnie rozpoznawany. Najłatwiej zainstalować CMake bezpośrednio ze strony projektu.)

## Instrukcja kompilacji
Mając zainstalowane wymagane narzędzia, najlepiej wykonać instalację z poziomu powłoki (*shell*) systemu operacyjnego.

Najpierw należy pobrać źródła projektu. Najwygodniej jest wykonać poniższą komendę - pobierze ona automatyczne projekt i zależności:

```
git clone --recurse-submodules https://github.com/wut-shogi/ShogiServer.git
```

Poniższa komenda automatycznie skompiluje projekt i jego zależności:

```
dotnet build
```

## Instrukcja uruchomienia

W celu uruchomienia serwera najlepiej użyć komendy:

```
dotnet run
```
