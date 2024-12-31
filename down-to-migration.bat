cd ./Src/
dotnet ef database update %1 --startup-project App --project Persistence --context %2
cd ..