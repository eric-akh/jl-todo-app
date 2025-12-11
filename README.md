# Todo App (Angular + .NET 9 Web API)

A simple TODO list application built as part of a technical assessment.  
Frontend is built with the latest version of Angular, and backend is a .NET Web API using in-memory storage.

## Features

- View TODO items
- Add new TODO items
- Delete TODO items
- Backend stores data in memory (no database required)
- Clean, simple structure following best practices

---

## Technologies Used

### Frontend
- Angular (latest)
- TypeScript
- HttpClient for API communication

### Backend
- .NET Web API (latest)
- Minimal APIs
- In-memory data store
- CORS enabled for Angular client

---

## Project Structure

root
├─ api/ → .NET Web API

│ ├─ Models/

│ ├─ Services/

│ ├─ Program.cs

│ └─ TodoApi.csproj

│

└─ client/ → Angular application

├─ src/app/

│ ├─ app.component.*

│ ├─ todo.service.ts

│ ├─ todo.ts

│ └─ app.ts (module/bootstrap)

├─ package.json

└─ angular.json

---

## How to Run the Project

### 1. Run the API
cd api
dotnet restore
dotnet run

API will be available at:
http://localhost:5028

Swagger UI:
http://localhost:5028/swagger

---

### 2. Run the Angular Client
cd client
npm install
npm start

Client will run at:
http://localhost:4200

---

## API Endpoints

| Method | Endpoint           | Description        |
|--------|---------------------|--------------------|
| GET    | /api/todos          | Get all TODO items |
| POST   | /api/todos          | Add new item       |
| DELETE | /api/todos/{id}     | Delete item        |

---

## Notes

- No database is used (per the requirements).
- Data resets when the API restarts.
- Make sure both API and Angular client are running simultaneously.
- No special setup required beyond restoring packages.

---

## Contact
If you have any issues running the app, feel free to contact me.
