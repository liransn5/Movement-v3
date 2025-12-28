# DataRetriever Application

This project provides a data retrieval API with caching support (Redis + SDCS) and SQL Server integration. The application is designed to run in Docker.

## Prerequisites

Before running the application, ensure you have the following installed and configured:
- **Docker Desktop**  
- **WSL 2**  
- **SQL Server**  
- **Ports**  
  - API: 8080  
  - Redis: 6379  
  Make sure these ports are not blocked by a firewall or used by other services.

---

## Getting Started

1. **Clone the repository**
git clone https://github.com/liransn5/Movement-v3.git

2. Update the connection string in `appsettings.json` to match your SQL Server credentials:
```json
"ConnectionStrings": {
  "DataRetriever": "Server=host.docker.internal;Database=DataRetrieverDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=False;"
}

3. Navigate to the root of the repository and run the batch script
``
<Path-to-repo-root>\DataRetriever\DataRetriever\RunAll.bat
