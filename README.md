# Notes App

A modern full-stack notes application with secure authentication.

## Features

- 🔐 Secure user authentication with JWT
- 📝 Create, edit, and delete notes
- 🔍 Search and sort notes
- 📱 Responsive design
- ✨ Clean, modern UI

## Tech Stack

**Backend:** ASP.NET Core 8, SQLite, JWT  
**Frontend:** Vue 3, TypeScript, Tailwind CSS

## Quick Start

### Prerequisites

- .NET 8 SDK
- Node.js 18+

### Run Locally

1. **Start Backend:**

```bash
cd backend/NotesAPI
dotnet run
```

2. **Start Frontend:**

```bash
cd frontend/notes-app
npm install
npm run dev
```

3. Visit `http://localhost:5174`

## Deployment

- **Backend:** Railway → [Guide](DEPLOY_NOW.md)
- **Frontend:** Vercel → [Guide](DEPLOY_NOW.md)

## License

MIT### Notes

- GET /api/notes - Get all notes for logged-in user
- POST /api/notes - Create a new note
- PUT /api/notes/{id} - Update an existing note
- DELETE /api/notes/{id} - Delete a note

## Security Features

- Passwords are hashed using BCrypt before storage
- JWT tokens with 15-minute expiration for access tokens
- Refresh tokens valid for 7 days
- Automatic token rotation on refresh
- Users can only access their own notes
- CORS configured for development

## Database

The application uses SQLite, a file-based database that requires no separate server installation. The database file (notes.db) is created automatically when you first run the backend.

Tables:

- Users - User account information
- Notes - User notes
- RefreshTokens - Token management for authentication

## Development

The frontend uses Vite for fast development with hot module replacement. The backend uses ASP.NET Core's built-in development server with automatic recompilation.

To build for production:

Backend:

```bash
dotnet publish -c Release
```

Frontend:

```bash
npm run build
```

## Common Issues

If you can't connect to the backend, make sure:

- The backend is running on port 5000
- CORS is configured to allow requests from localhost:5173

If you have authentication issues:

- Clear your browser's session storage
- Make sure the JWT key in appsettings.json is at least 32 characters

## License

This project was created for demonstration purposes.
