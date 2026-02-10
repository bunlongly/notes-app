FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore
COPY NotesAPI.csproj ./
RUN dotnet restore NotesAPI.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish NotesAPI.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "NotesAPI.dll"]
