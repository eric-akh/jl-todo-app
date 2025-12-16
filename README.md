# Todo App (Angular + .NET Web API)

A simple TODO list application built as part of a technical assessment.

The backend is implemented using the latest .NET Web API with in-memory storage and automated tests.  
The frontend is built with the latest Angular version and Angular Material.

---

## Features

- View TODO items
- Add new TODO items with validation
- Delete TODO items with confirmation
- Mark TODO items as completed
- Due date is mandatory (validated on backend and frontend)
- Backend stores data in memory (no database required)
- Unit and integration tests on backend
- Unit tests on frontend
- Clean, readable structure following best practices

---

## Technologies Used

### Backend
- .NET Web API (latest, Minimal APIs)
- C#
- In-memory data store
- Data validation using DTOs
- Unit tests (xUnit)
- Integration tests using WebApplicationFactory

### Frontend
- Angular (latest)
- TypeScript
- Angular Material
- Reactive Forms
- HttpClient for API communication
- Unit tests with `ng test`

---

## Project Structure

root
├─ api/ → .NET Web API
│ ├─ Models/
│ ├─ Services/
│ ├─ Program.cs
│ ├─ TodoApi.csproj
│ └─ TodoApi.Tests/ → Unit & integration tests
│
└─ client/ → Angular application
├─ todo-client/
│ ├─ src/app/
│ │ ├─ app.component.*
│ │ ├─ confirm-dialog.component.*
│ │ ├─ todo.service.ts
│ │ └─ todo.ts
│ ├─ package.json
│ └─ angular.json


---

## How to Run the Project

### 1. Run the Backend API

```
cd api
dotnet restore
dotnet run
API will be available at:

http://localhost:5028
Swagger UI:


http://localhost:5028/swagger
Run Backend Tests


cd api
dotnet test

2. Run the Angular Client

cd client/todo-client
npm install
npm start
Client will be available at:

http://localhost:4200
Run Frontend Tests
npm test
API Endpoints
Method	Endpoint	Description
GET	/api/todos	Get all TODO items
POST	/api/todos	Add a new TODO item
PATCH	/api/todos/{id}/toggle	Toggle completion status
DELETE	/api/todos/{id}	Delete a TODO item

Validation Rules
Title is required and trimmed

Due date is mandatory (enforced on backend)

Priority must be a valid enum value

Invalid requests return appropriate HTTP status codes

Notes
No database is used (per assessment requirements)

Data resets when the API restarts

Backend validation ensures data integrity even if client-side validation is bypassed

UI focuses on clarity and usability rather than visual complexity

