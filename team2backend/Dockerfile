FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . .

RUN dotnet publish team2backend.csproj -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /src
COPY --from=build-env /src/publish .

CMD ASPNETCORE_URLS=http://*:$PORT dotnet team2backend.dll