#!/bin/bash

# Notes Application - Quick Start Script
# This script starts both backend and frontend services

echo "================================================"
echo " Starting Notes Application"
echo "================================================"
echo ""

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Get script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Check prerequisites
command -v dotnet &> /dev/null || { echo ".NET SDK not found. Run: brew install dotnet"; exit 1; }
command -v node &> /dev/null || { echo "Node.js not found. Install from nodejs.org"; exit 1; }

echo -e "${GREEN}✓ Prerequisites found${NC}"
echo ""

# Start backend in background
echo "🔧 Starting Backend (ASP.NET Core)..."
cd "$SCRIPT_DIR/backend/NotesAPI"
dotnet run > /dev/null 2>&1 &
BACKEND_PID=$!
echo -e "${GREEN}✓ Backend started (PID: $BACKEND_PID) - http://localhost:5000${NC}"

# Wait for backend to be ready
sleep 3

# Start frontend in background
echo "🎨 Starting Frontend (Vue + Vite)..."
cd "$SCRIPT_DIR/frontend/notes-app"
npm run dev > /dev/null 2>&1 &
FRONTEND_PID=$!
echo -e "${GREEN}✓ Frontend started (PID: $FRONTEND_PID) - http://localhost:5173${NC}"

echo ""
echo "================================================"
echo -e "${YELLOW}✨ Application is ready!${NC}"
echo ""
echo "  Frontend: http://localhost:5173"
echo "  Backend:  http://localhost:5000"
echo ""
echo "Press Ctrl+C to stop all services"
echo "================================================"

# Cleanup function
cleanup() {
    echo ""
    echo "Stopping services..."
    kill $BACKEND_PID $FRONTEND_PID 2>/dev/null
    echo "✓ Services stopped"
    exit 0
}

trap cleanup INT TERM

# Keep script running
wait
