# Metasign

An e-contract platform for businesses and individuals to create, deliver, sign, store, and manage contracts digitally.

## Features

- Create and manage different types of contracts
- Deliver contracts for signing via online channels (email, link)
- Electronic & digital signatures
- Batch contract signing
- Contract tracking, reporting, and history
- Google OAuth login

---

## Architecture

**Single Docker image** serving both the Angular frontend (embedded in `wwwroot`) and the .NET 6 backend. Deployed on **Google Cloud Run**, with **Supabase** (PostgreSQL) as the database.

```
┌─────────────────────────────────────────┐
│           Google Cloud Run              │
│  ┌──────────────────────────────────┐   │
│  │  .NET 6 Web Host (port 8080)     │   │
│  │   ├── API endpoints              │   │
│  │   └── Serves Angular (wwwroot)   │   │
│  └──────────────────────────────────┘   │
└─────────────────────────────────────────┘
              │
              ▼
     Supabase (PostgreSQL)
```

---

## Prerequisites

| Tool | Version |
| :--- | :--- |
| [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) | 6.0 |
| [Node.js](https://nodejs.org/) | 18.x |
| [Docker](https://www.docker.com/get-started) | 20+ |
| [Angular CLI](https://angular.io/cli) | 12.x |
| Supabase project | (cloud) |
| Google Cloud project | (for Cloud Run deployment) |

---

## Local Development

### 1. Clone the repository

```bash
git clone https://github.com/ncc-erp/metasign.git
cd metasign
```

### 2. Configure the Backend

Edit `aspnet-core/src/EC.Web.Host/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=<your-supabase-host>;Port=5432;Database=postgres;Username=postgres.<project-ref>;Password=<your-password>;SSL Mode=Require;Trust Server Certificate=true;"
  },
  "App": {
    "ServerRootAddress": "http://localhost:44311/",
    "ClientRootAddress": "http://localhost:4200/",
    "CorsOrigins": "http://localhost:4200"
  }
}
```

### 3. Run the Backend

```bash
cd aspnet-core
dotnet run --project src/EC.Web.Host/EC.Web.Host.csproj
```

Default backend URL: `http://localhost:44311/`

### 4. Run the Frontend

```bash
cd angular
npm install
npm start
```

Default frontend URL: `http://localhost:4200/`

---

## Docker (Local)

Build and run the full stack as a single container:

```bash
docker build -t metasign .
docker run -p 8080:8080 \
  -e "ConnectionStrings__Default=<your-supabase-connection-string>" \
  metasign
```

Open `http://localhost:8080`

---

## Google Cloud Run Deployment

CI/CD is handled by **Google Cloud Build** using `cloudbuild.yaml`.

### Manual Deploy

```bash
# Build and push image
gcloud builds submit --config cloudbuild.yaml \
  --substitutions _REGION=us-central1,_REPO_NAME=metasign-repo,_SERVICE_NAME=metasign,_DB_CONNECTION_STRING="<your-supabase-connection-string>"
```

### Cloud Build Trigger (Automated)

Set up a trigger in the [Google Cloud Console](https://console.cloud.google.com/cloud-build/triggers) pointing to your repository, using `cloudbuild.yaml`.

Required substitution variables:
| Variable | Description |
| :--- | :--- |
| `_REGION` | GCP region (e.g. `us-central1`) |
| `_REPO_NAME` | Artifact Registry repository name |
| `_SERVICE_NAME` | Cloud Run service name |
| `_DB_CONNECTION_STRING` | Supabase PostgreSQL connection string |

---

## Authentication

- **Username/Password** (default)
- **Google OAuth** — configure in `appsettings.json` under `Authentication:Google`

---

## Screenshots

![MetaSign1](_screenshots/MetaSign1.png)
![MetaSign2](_screenshots/MetaSign2.png)
![MetaSign3](_screenshots/MetaSign3.png)