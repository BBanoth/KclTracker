@echo Building database project
pushd src\Database\Sql
dotnet build
pushd bin\Debug\net5.0
@echo Cleaning database
dotnet KclTracker.Database.Sql.dll CleanDatabase
popd
popd
@echo Building UserManagement project
pushd tools\src\UserManagement
dotnet build
pushd bin\Debug\net5.0
@echo Seeding users and roles
dotnet KclTracker.Tools.UserManagement.dll SeedUsersAndRoles
popd
popd
pause