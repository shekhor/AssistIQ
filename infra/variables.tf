variable "environment" {
  description = "Environment name"
  type        = string
  default     = "prod"
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "canadacentral"
}

variable "project_name" {
  description = "Project name used for naming resources"
  type        = string
  default     = "assistiq"
}

variable "db_admin_username" {
  description = "Database administrator username"
  type        = string
  default     = "sqladmin"
}

variable "db_admin_password" {
  description = "Database administrator password"
  type        = string
  sensitive   = true
}

variable "static_web_app_location" {
  description = "Location for Static Web App (limited region availability)"
  type        = string
  default     = "eastus2"
}