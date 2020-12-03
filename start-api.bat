@echo Building database project
pushd src\Database\Sql
dotnet build
pushd bin\Debug\net5.0
@echo Cleaning database
dotnet KclTracker.Database.Sql.dll CleanDatabase
popd
popd
pushd src\facade
dotnet run --project KclTracker.Services.Facade.csproj --urls https://localhost:44358