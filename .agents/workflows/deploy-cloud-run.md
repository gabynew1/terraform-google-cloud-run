---
description: Build and deploy Metasign to Google Cloud Run
---

# Deploy to Google Cloud Run

This workflow supports both manual and automated (GitHub-triggered) deployments using Google Cloud Build.

## Option A: Automated GitHub CI/CD (Recommended)

Metasign is configured with a **Cloud Build Trigger** connected to your GitHub repository.

1. **Trigger Action**: Push your changes to the `main` branch.
```bash
git push origin main
```

2. **Automated Steps**: Cloud Build will automatically:
    - Build the container from `Dockerfile`.
    - Run database migrations (`EC.Migrator`).
    - Deploy the new image to Cloud Run.

## Option B: Manual Deployment (Ad-hoc)

Use this if you need to deploy a specific local state without pushing to GitHub.

// turbo-all
1. Ensure you are logged in to gcloud:
```bash
gcloud auth login
```

2. Set your active project:
```bash
gcloud config set project [YOUR_PROJECT_ID]
```

3. Submit the build manually:
```bash
gcloud builds submit --config cloudbuild.yaml \
  --substitutions _REGION=us-central1,_REPO_NAME=metasign-repo,_SERVICE_NAME=metasign,_DB_CONNECTION_STRING="[YOUR_SUPABASE_CONNECTION_STRING]"
```

> [!IMPORTANT]
> Replace `[YOUR_PROJECT_ID]` and `[YOUR_SUPABASE_CONNECTION_STRING]` with your actual values.
