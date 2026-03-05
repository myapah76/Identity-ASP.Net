# Identity Service
Built with **ASP.NET Core (.NET 8)** following **Clean Architecture** principles and using **JWT authentication**.

The service manages **users, roles, and authentication**, and integrates with **PostgreSQL** for persistent storage and **Redis** for caching and session management.

---

# Features

## Authentication & Authorization
- JWT-based authentication (Access Token & Refresh Token)
- Role-based access control (RBAC)
- Secure password hashing
- Token expiration and refresh support

## User Management
- Create and manage user accounts
- Update and delete users
- Retrieve user information
- Role assignment support

## Role Management
- Create roles
- Update roles
- Delete roles
- List available roles

## Infrastructure
- PostgreSQL for persistent data storage
- Redis for caching and session handling
- Docker support for containerized deployment
- Environment-based configuration

---

# Tech Stack

## Backend
- ASP.NET Core (.NET 8)
- Clean Architecture

## Database
- PostgreSQL  
- Redis (caching)

## Tools
- Docker / Docker Compose  
- Git

---

# Setup Instructions

## 1. Clone the Repository
- git clone <repository-url>
- cd identity-service
## 2. Configure Environment Variables
- Copy the example environment file:
- cp .env.example .env

- Example configuration:

- DB_USER=admin
- DB_PASSWORD=12345
- DB_NAME=identity
- DB_PORT=5432

- REDIS_PORT=6379
## 3. Run with Docker
- Start PostgreSQL and Redis:
- docker compose up -d
## 4. Run the Application
- dotnet run
