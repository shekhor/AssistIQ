terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# Get current Azure client config
data "azurerm_client_config" "current" {}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "rg-${var.project_name}-${var.environment}"
  location = var.location
}

# App Service Plan (for .NET backend)
resource "azurerm_service_plan" "backend" {
  name                = "asp-${var.project_name}-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "P1v3"
}

# .NET Backend App Service
resource "azurerm_linux_web_app" "backend" {
  name                = "app-${var.project_name}-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.backend.id

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
    cors {
      allowed_origins = [
        "https://${var.project_name}-${var.environment}.azurestaticapps.net"
      ]
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = var.environment
    "WEBSITE_RUN_FROM_PACKAGE" = "1"
  }

  identity {
    type = "SystemAssigned"
  }
}

# React Frontend — Static Web App
resource "azurerm_static_web_app" "frontend" {
  name                = "swa-${var.project_name}-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku_tier            = "Standard"
}

# Azure SQL Server
resource "azurerm_mssql_server" "main" {
  name                         = "sql-${var.project_name}-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  location                     = azurerm_resource_group.main.location
  version                      = "12.0"
  administrator_login          = var.db_admin_username
  administrator_login_password = var.db_admin_password
}

# Azure SQL Database
resource "azurerm_mssql_database" "main" {
  name      = "db-${var.project_name}-${var.environment}"
  server_id = azurerm_mssql_server.main.id
  sku_name  = "S1"
}

# SQL Firewall rule — allow Azure services
resource "azurerm_mssql_firewall_rule" "azure_services" {
  name             = "allow-azure-services"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# Key Vault
resource "azurerm_key_vault" "main" {
  name                = "kv-${var.project_name}-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get", "List", "Set", "Delete"
    ]
  }

  # Allow the backend app to read secrets
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = azurerm_linux_web_app.backend.identity[0].principal_id

    secret_permissions = [
      "Get", "List"
    ]
  }
}

# Store DB connection string in Key Vault
resource "azurerm_key_vault_secret" "db_connection" {
  name         = "db-connection-string"
  value        = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Database=${azurerm_mssql_database.main.name};User ID=${var.db_admin_username};Password=${var.db_admin_password};Encrypt=true;"
  key_vault_id = azurerm_key_vault.main.id
}

# Application Insights
resource "azurerm_application_insights" "main" {
  name                = "ai-${var.project_name}-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  application_type    = "web"
}