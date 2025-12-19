# Expense Sharing Application (.NET + Angular)

A comprehensive expense sharing application similar to Splitwise, built with **.NET 8 Web API** backend and **Angular** frontend.

## Technology Stack

### Backend
- **.NET 8** Web API
- **Entity Framework Core** with SQL Server
- **SQL Server** (using server: LIN-5CG1153CJ0)

### Frontend
- **Angular 17**
- **TypeScript**
- **SCSS** for styling

## Quick Start

### Prerequisites
- **.NET 8 SDK**
- **Node.js 18+** and npm
- **SQL Server** (LIN-5CG1153CJ0)

### Option 1: Use Batch File
```bash
start-app.bat
```

### Option 2: Manual Start

**Backend:**
```bash
dotnet run
```

**Frontend:**
```bash
cd expense-frontend
npm install
npm start
```

### Access
- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

## Database Setup

The database will be **automatically created** when you first run the application.

1. **Ensure SQL Server is running** on LIN-5CG1153CJ0
2. **Run the application** - database and tables auto-create
3. **Optional**: Add sample data using the provided SQL script

## API Endpoints

All endpoints remain the same as the Java version:

- `GET /api/users` - Get all users
- `POST /api/users` - Create user
- `GET /api/groups` - Get all groups
- `POST /api/groups` - Create group
- `GET /api/expenses/group/{groupId}` - Get expenses
- `POST /api/expenses` - Create expense
- `GET /api/balances/group/{groupId}` - Get balances
- `POST /api/balances/settle` - Settle debt

## Features

✅ **All features from Java version**:
- User Management
- Group Management  
- Expense Tracking (Equal/Exact/Percentage splits)
- Balance Calculations
- Debt Settlement
- Simplified debt suggestions

## Development

**Run in development:**
```bash
dotnet run --environment Development
```

**Database migrations:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

The .NET backend provides the same functionality as the Java version with improved performance and native Windows integration!