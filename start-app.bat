@echo off
echo Starting Expense Sharing Application (.NET + Angular)...
echo.

echo Starting Backend (.NET Web API)...
start "Backend" cmd /k "cd /d %~dp0 && dotnet run"

echo Waiting for backend to start...
timeout /t 10 /nobreak > nul

echo Starting Frontend (Angular)...
start "Frontend" cmd /k "cd /d %~dp0expense-frontend && npm start"

echo.
echo Both services are starting...
echo Backend: http://localhost:5000 (or https://localhost:5001)
echo Frontend: http://localhost:4200
echo.
echo Press any key to exit...
pause > nul