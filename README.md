# Notes Application

A full-stack web application for managing personal notes with secure user authentication.

## Features

- User registration and login with JWT authentication
- Create, edit, view, and delete notes
- Search notes by title or content
- Sort notes by date or title
- Each user can only access their own notes
- Automatic token refresh for seamless experience
- Responsive design that works on mobile and desktop

## Technology Stack

### Backend

- ASP.NET Core 8.0
- SQLite database with Dapper ORM
- JWT for authentication
- BCrypt for password hashing

### Frontend

- Vue 3 with Composition API
- TypeScript
- Tailwind CSS
- Pinia for state management
- Axios for API calls
- Vite as build tool

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18 or higher

### Running the Application

1. Start the backend:

```bash
cd backend/NotesAPI
dotnet restore
dotnet run
```

Backend will run on http://localhost:5000

2. Start the frontend (in a new terminal):

```bash
cd frontend/notes-app
npm install
npm run dev
```

Frontend will run on http://localhost:5173

3. Open your browser to http://localhost:5173

### Quick Start Script

You can also use the provided start script:

```bash
chmod +x start.sh
./start.sh
```

## Project Structure

```
backend/
  NotesAPI/
    Controllers/     - API endpoints
    Data/           - Database connection
    Models/         - Data models
    Services/       - Business logic

frontend/
  notes-app/
    src/
      views/        - Page components
      stores/       - State management
      services/     - API integration
      components/   - Reusable UI components
      router/       - Navigation routes
```

## API Endpoints

### Authentication

- POST /api/auth/register - Create a new account
- POST /api/auth/login - Login to existing account
- POST /api/auth/refresh - Refresh access token

### Notes

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
