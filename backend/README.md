# Notes API - Backend

ASP.NET Core 8 REST API with SQLite database.

## Setup

```bash
cd backend/NotesAPI
dotnet restore
dotnet run
```

API runs on `http://localhost:5000`

## Environment Variables

- `Jwt__Key` - JWT signing key
- `ConnectionStrings__DefaultConnection` - Database path

## Deploy to Railway

Set root directory to `backend/NotesAPI` in Railway settings.
