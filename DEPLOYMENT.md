# GitHub Deployment Guide

## Backend Deployment (Railway - Free)

### 1. Install Railway CLI
```bash
npm install -g @railway/cli
```

### 2. Login to Railway
```bash
railway login
```

### 3. Initialize Project
```bash
cd Split-Backend
railway init
```

### 4. Deploy Backend
```bash
railway up
```

### 5. Get Backend URL
```bash
railway domain
# Copy the URL (e.g., https://your-app.railway.app)
```

---

## Frontend Deployment (GitHub Pages - Free)

### 1. Update API URL in Angular
Edit `expense-frontend/src/environments/environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-app.railway.app/api'  // Your Railway URL
};
```

### 2. Build Angular for Production
```bash
cd expense-frontend
npm install
ng build --configuration production --base-href="/Split-Backend/"
```

### 3. Deploy to GitHub Pages
```bash
# Install gh-pages
npm install -g angular-cli-ghpages

# Deploy
npx angular-cli-ghpages --dir=dist/expense-frontend
```

### 4. Enable GitHub Pages
1. Go to GitHub repo → Settings → Pages
2. Source: `gh-pages` branch
3. Your site: `https://yourusername.github.io/Split-Backend/`

---

## Alternative: Render (Backend)

### 1. Push to GitHub
```bash
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/yourusername/Split-Backend.git
git push -u origin main
```

### 2. Deploy on Render
1. Go to https://render.com
2. New → Web Service
3. Connect GitHub repo
4. Build Command: `dotnet publish -c Release -o out`
5. Start Command: `dotnet out/ExpenseSharing.dll`
6. Deploy

---

## Alternative: Netlify (Frontend)

### 1. Build Angular
```bash
cd expense-frontend
ng build --configuration production
```

### 2. Deploy to Netlify
```bash
npm install -g netlify-cli
netlify deploy --prod --dir=dist/expense-frontend
```

---

## Update Backend URL in Frontend

After deploying backend, update Angular service:

**expense-frontend/src/app/services/user.service.ts** (and all services):
```typescript
private apiUrl = 'https://your-backend-url.railway.app/api/users';
```

---

## Quick Commands Summary

```bash
# Backend (Railway)
railway login
railway init
railway up
railway domain

# Frontend (GitHub Pages)
cd expense-frontend
ng build --configuration production --base-href="/Split-Backend/"
npx angular-cli-ghpages --dir=dist/expense-frontend

# Access your app
# Frontend: https://yourusername.github.io/Split-Backend/
# Backend: https://your-app.railway.app
```

---

## Free Hosting Limits

**Railway**: 500 hours/month, $5 credit
**GitHub Pages**: Unlimited static hosting
**Render**: 750 hours/month free tier

**Total Cost: $0**
