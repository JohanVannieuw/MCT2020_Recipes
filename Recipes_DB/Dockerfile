#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Recipes_DB/Recipes_DB.csproj", "Recipes_DB/"]
RUN dotnet restore "Recipes_DB/Recipes_DB.csproj"
COPY . .

COPY ["RecipesDB_UnitTests/RecipesDB_UnitTests.csproj","RecipesDB_UnitTests/"]
RUN dotnet restore "RecipesDB_UnitTests/RecipesDB_UnitTests.csproj" 

#4.RUN DE UNITTESTEN ZONDER BUILD (plaats in commentaar) 
#RUN dotnet build "RecipeAPI.UnitTests.csproj" -c Release -o /app/bui 
#FROM build AS unittest
#WORKDIR "/src/Recipe_DB"
RUN dotnet test "RecipesDB_UnitTests/RecipesDB_UnitTests.csproj" --logger:trx



WORKDIR "/src/Recipes_DB"
RUN dotnet build "Recipes_DB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Recipes_DB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Recipes_DB.dll"]
