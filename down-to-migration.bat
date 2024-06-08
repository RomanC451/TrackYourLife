cd ./src/
dotnet ef database update %1 --startup-project App --project Persistence --context ApplicationWriteDbContext
cd ..