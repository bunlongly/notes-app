# Notes Application - Backend

ASP.NET Core Web API for Notes Application with SQL Server and Dapper.

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or full version)
- Visual Studio Code or Visual Studio 2022

## Setup Instructions

### 1. Database Setup

Run the SQL script to create the database:

```bash
# Using sqlcmd (if installed)
sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -i Database/setup.sql

# Or use SQL Server Management Studio (SSMS) or Azure Data Studio
# Open Database/setup.sql and execute it
```

**Note:** Update the connection string in `appsettings.json` with your SQL Server credentials.

### 2. Install Dependencies

```bash
cd backend/NotesAPI
dotnet restore
```

### 3. Run the Application

```bash
dotnet run
```

The API will be available at: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Notes (Requires Authentication)

- `GET /api/notes` - Get all notes for current user
- `GET /api/notes/{id}` - Get specific note
- `POST /api/notes` - Create new note
- `PUT /api/notes/{id}` - Update note
- `DELETE /api/notes/{id}` - Delete note

## Project Structure

```
NotesAPI/
├── Controllers/          # API endpoints
├── Data/                 # Database connection factory
├── Models/              # Data models and DTOs
├── Services/            # Business logic
└── Program.cs           # Application entry point
```
