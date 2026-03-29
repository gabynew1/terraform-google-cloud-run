variable "project_id" {
  description = "The GCP project ID"
  type        = string
  default     = "greencrm-491022"
}

variable "region" {
  description = "The GCP region to deploy to"
  type        = string
  default     = "us-central1"
}

variable "service_name" {
  description = "The name of the Cloud Run service"
  type        = string
  default     = "one-cloud-meta"
}

variable "gcs_bucket_name" {
  description = "The name of the GCS bucket"
  type        = string
  default     = "one-cloud-meta-storage"
}

variable "image_tag" {
  description = "The Docker image tag to deploy"
  type        = string
  default     = "latest"
}

variable "github_owner" {
  description = "The GitHub username or organization"
  type        = string
}
