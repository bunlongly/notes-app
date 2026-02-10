# Notes Application - Frontend

Vue 3 + TypeScript + Tailwind CSS frontend for the Notes Application.

## Prerequisites

- Node.js 18+ and npm

## Setup Instructions

### 1. Install Dependencies

```bash
cd frontend/notes-app
npm install
```

### 2. Configure API URL

The API URL is configured in `src/services/api.ts`. By default, it points to `http://localhost:5000/api`.

If your backend runs on a different port, update the `API_URL` constant.

### 3. Run Development Server

```bash
npm run dev
```

The application will be available at: `http://localhost:5173`

### 4. Build for Production

```bash
npm run build
```

## Features

- **Authentication**: Login and register with JWT
- **Notes Management**: Full CRUD operations
- **Search**: Real-time search across notes
- **Filtering & Sorting**: Sort by date or title, ascending or descending
- **Responsive Design**: Works on mobile, tablet, and desktop
- **State Management**: Pinia for reactive state
- **Modern UI**: Tailwind CSS for styling

## Project Structure

```
src/
├── components/          # Reusable components
├── views/              # Page components
├── stores/             # Pinia state management
├── services/           # API service layer
├── types/              # TypeScript interfaces
├── router/             # Vue Router configuration
└── style.css           # Global styles with Tailwind
```

## Default Test Account

You can create a new account using the Register page, or use these test credentials if you've already created them:

- Email: test@example.com
- Password: password123
