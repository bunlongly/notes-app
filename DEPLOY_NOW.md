# Quick Deployment Guide

## ✅ Your Domains

- **Frontend (Vercel)**: https://notes-app-tech.vercel.app
- **Backend (Railway)**: https://notes-app-production-3416.up.railway.app

## 🚀 Deploy to Railway (Backend)

### 1. Set Environment Variables in Railway

Go to your Railway project → Variables tab and add:

```
ASPNETCORE_ENVIRONMENT=Production
Jwt__Key=ThisIsASecretKeyForJWT12345678901234567890_ChangeThisInProduction
ConnectionStrings__DefaultConnection=Data Source=/app/notes.db
```

**IMPORTANT**: Change the `Jwt__Key` to a secure random string (at least 32 characters) for production!

### 2. Set Watch Paths (Optional but Recommended)

In Railway Settings, set watch path to:

```
/backend/**
```

This ensures Railway only redeploys when backend code changes.

### 3. Deploy

Push your changes to GitHub:

```bash
git add .
git commit -m "Update CORS and deployment configs"
git push
```

Railway will automatically redeploy!

## 🌐 Deploy to Vercel (Frontend)

### 1. Set Environment Variables in Vercel

Go to your Vercel project → Settings → Environment Variables and add:

**Name**: `VITE_API_URL`  
**Value**: `https://notes-app-production-3416.up.railway.app/api`  
**Environments**: Select "Production" (and optionally Preview)

### 2. Redeploy

After setting the environment variable:

- Go to Deployments tab
- Click on the latest deployment → "..." menu → "Redeploy"

Or push a new commit to trigger deployment:

```bash
git add .
git commit -m "Update API URL for production"
git push
```

## 🧪 Test Your Deployment

1. **Test Backend Health**:

   ```bash
   curl https://notes-app-production-3416.up.railway.app/
   ```

   Should return: "Notes API is running"

2. **Test Frontend**:
   - Visit: https://notes-app-tech.vercel.app
   - Try to register a new account
   - Login and create a note

## 🔒 Security Checklist

- [ ] Change the JWT key in Railway to a secure random value
- [ ] Verify CORS is working (no console errors)
- [ ] Test full registration/login flow
- [ ] Verify notes CRUD operations work

## 📝 Current Status

✅ Backend code updated with correct Vercel domain  
✅ Frontend configured with correct Railway URL  
✅ CORS configured for both localhost and production  
⏳ Need to push changes to GitHub  
⏳ Need to set Railway environment variables  
⏳ Need to set Vercel environment variable

## 🐛 Troubleshooting

**If you see CORS errors**:

- Verify the Vercel URL is exactly `https://notes-app-tech.vercel.app` in Program.cs
- Make sure Railway has redeployed after the changes
- Check Railway logs for errors

**If registration/login fails**:

- Check Railway environment variables are set
- Verify the JWT key is set and matches in all environments
- Check Railway logs for detailed error messages

**If frontend can't connect to backend**:

- Verify VITE_API_URL in Vercel matches your Railway URL
- Make sure the Railway URL ends with `/api`
- Check browser console for network errors
