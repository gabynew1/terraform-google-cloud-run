terraform {
  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "~> 5.0"
    }
  }
}

provider "google" {
  project = var.project_id
  region  = var.region
}

# Enable APIs
resource "google_project_service" "run" {
  service            = "run.googleapis.com"
  disable_on_destroy = false
}

resource "google_project_service" "registry" {
  service            = "artifactregistry.googleapis.com"
  disable_on_destroy = false
}

resource "google_project_service" "build" {
  service            = "cloudbuild.googleapis.com"
  disable_on_destroy = false
}

# Cloud Run Service
resource "google_cloud_run_v2_service" "default" {
  depends_on = [google_project_service.run]
  name       = var.service_name
  location = var.region
  ingress  = "INGRESS_TRAFFIC_ALL"

  template {
    containers {
      image = "${var.region}-docker.pkg.dev/${var.project_id}/metasign-repo/${var.service_name}:${var.image_tag}"
      
      ports {
        container_port = 8080
      }

      resources {
        limits = {
          cpu    = "1"
          memory = "2Gi"
        }
      }
    }
  }
}

# Allow specific user access
resource "google_cloud_run_v2_service_iam_member" "noauth" {
  location = google_cloud_run_v2_service.default.location
  name     = google_cloud_run_v2_service.default.name
  role     = "roles/run.invoker"
  member   = "user:gabriel@zealot.ro"
}

# Google Cloud Storage Bucket
resource "google_storage_bucket" "storage" {
  name          = var.gcs_bucket_name
  location      = var.region
  force_destroy = true

  uniform_bucket_level_access = true
}

# Grant Cloud Run Service Account access to the bucket
resource "google_storage_bucket_iam_member" "run_storage_access" {
  bucket = google_storage_bucket.storage.name
  role   = "roles/storage.objectAdmin"
  member = "serviceAccount:${data.google_project.project.number}-compute@developer.gserviceaccount.com"
}

# Artifact Registry Repository
resource "google_artifact_registry_repository" "repo" {
  depends_on    = [google_project_service.registry]
  location      = var.region
  repository_id = "metasign-repo"
  description   = "Docker repository for Metasign"
  format        = "DOCKER"
}

# Cloud Build Trigger
resource "google_cloudbuild_trigger" "github-trigger" {
  name     = "metasign-github-trigger"
  location = var.region

  github {
    owner = var.github_owner
    name  = "terraform-google-cloud-run"
    push {
      branch = "^main$" # or "^dev$"
    }
  }

  filename = "cloudbuild.yaml"

  substitutions = {
    _REGION       = var.region
    _REPO_NAME    = google_artifact_registry_repository.repo.repository_id
    _SERVICE_NAME = var.service_name
  }

  include_build_logs = "INCLUDE_BUILD_LOGS_WITH_STATUS"
}

# IAM Role for Cloud Build to deploy to Cloud Run
resource "google_project_iam_member" "cloudbuild_run_admin" {
  project = var.project_id
  role    = "roles/run.admin"
  member  = "serviceAccount:${data.google_project.project.number}@cloudbuild.gserviceaccount.com"
}

resource "google_project_iam_member" "cloudbuild_sa_user" {
  project = var.project_id
  role    = "roles/iam.serviceAccountUser"
  member  = "serviceAccount:${data.google_project.project.number}@cloudbuild.gserviceaccount.com"
}

data "google_project" "project" {}
