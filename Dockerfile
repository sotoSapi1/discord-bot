FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App
# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/StrikerBot.dll"]