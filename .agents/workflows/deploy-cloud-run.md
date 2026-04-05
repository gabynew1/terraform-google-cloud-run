---
description: Build and deploy Metasign to Google Cloud Run
---

# Deploy to Google Cloud Run

This workflow automates the build and deployment of the Metasign single-container application.

// turbo-all
1. Ensure you are logged in to gcloud:
```bash
gcloud auth login
```

2. Set your active project:
```bash
gcloud config set project [YOUR_PROJECT_ID]
```

3. Submit the build to Cloud Build:
```bash
gcloud builds submit --config cloudbuild.yaml \
  --substitutions _REGION=us-central1,_REPO_NAME=metasign-repo,_SERVICE_NAME=metasign,_DB_CONNECTION_STRING="[YOUR_SUPABASE_CONNECTION_STRING]"
```

> [!IMPORTANT]
> Replace `[YOUR_PROJECT_ID]` and `[YOUR_SUPABASE_CONNECTION_STRING]` with your actual values.
