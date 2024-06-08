cd ./src/
dotnet ef migrations add %1 --startup-project App --project Persistence --context ApplicationWriteDbContext
cd ..