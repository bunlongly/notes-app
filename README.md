# Notes App

A full-stack notes application built with ASP.NET Core 8 (backend) and Vue 3 + Vite (frontend).

## Features

- User registration and login (JWT authentication)
- Create, read, update, and delete notes (CRUD)
- Each note has:
  - Title (required)
  - Content (optional)
  - Created At (auto-generated)
  - Updated At (auto-generated when edited)
- Users can only access and modify their own notes
- Notes list page with filtering, sorting, and search
- Responsive UI using Tailwind CSS
- State management (Pinia)
- Simple API integration (Axios)
- CORS and environment variable support for production

## Tech Stack

- **Frontend:** Vue 3, TypeScript, Vite, Axios, Tailwind CSS
- **Backend:** ASP.NET Core 8, Dapper, JWT
- **Deployment:** Railway (backend), Vercel (frontend)

## Project Structure

```
notes-app-techbodia/
├── backend/
│   └── NotesAPI/
│       ├── Controllers/
│       ├── Data/
│       ├── Middleware/
│       ├── Models/
│       ├── Services/
│       ├── Program.cs
│       ├── NotesAPI.csproj
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── nixpacks.toml
│       └── ...
├── frontend/
│   └── notes-app/
│       ├── src/
│       ├── index.html
│       ├── package.json
│       ├── tailwind.config.js
│       ├── vite.config.ts
│       └── ...
```

## Setup & Deployment

### Backend (Railway)

1. Set root directory to `backend/NotesAPI` in Railway dashboard.
2. Use `nixpacks.toml` for build/start commands:
   - Build: `dotnet publish NotesAPI.csproj -c Release -o out`
   - Start: `dotnet out/NotesAPI.dll`
3. Add environment variables:
   - `Jwt__Key` (at least 16 chars)
   - `ConnectionStrings__DefaultConnection` 
4. Redeploy after any changes.

### Frontend (Vercel)

1. Set root directory to `frontend/notes-app` in Vercel dashboard.
2. Add environment variable:
   - `VITE_API_URL` (your Railway backend API URL, e.g. `https://notes-app-production-xxxx.up.railway.app/api`)
3. Redeploy after any changes.

## Usage

- Register a new account, log in, and manage your notes.
- All data is stored securely in the backend.

## Troubleshooting

- If you see CORS errors, make sure your backend's CORS policy includes your Vercel domain.
- If you see JWT errors, ensure your `Jwt__Key` is at least 16 characters.
- If the backend fails to start, check Railway build logs and environment variables.

## License

MIT
