FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything from current directory
COPY . .

# Restore and publish explicitly naming the project
RUN dotnet restore "./NotesAPI.csproj"
RUN dotnet publish "./NotesAPI.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "NotesAPI.dll"]
