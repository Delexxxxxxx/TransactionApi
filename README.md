# Transaction API

REST API for idempotent transaction creation and retrieval.

- .NET + EF Core  
- PostgreSQL  
- Docker  

**POST `/transactions`** — creates a transaction (same `id` is safe to retry)  
**GET `/transactions/{id}`** — retrieves a transaction  

Run:
```bash
docker-compose up --build
