# Resource Group
output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

# Backend API
output "backend_url" {
  description = "URL of the .NET backend API"
  value       = "https://${azurerm_linux_web_app.backend.default_hostname}"
}

output "backend_app_name" {
  description = "Name of the backend App Service"
  value       = azurerm_linux_web_app.backend.name
}

# Frontend
output "frontend_url" {
  description = "URL of the React frontend"
  value       = "https://${azurerm_static_web_app.frontend.default_host_name}"
}

output "frontend_api_key" {
  description = "Deployment token for Static Web App"
  value       = azurerm_static_web_app.frontend.api_key
  sensitive   = true
}

# Database
output "db_server_name" {
  description = "SQL Server name"
  value       = azurerm_mssql_server.main.name
}

output "db_server_fqdn" {
  description = "SQL Server fully qualified domain name"
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
}

# Key Vault
output "key_vault_name" {
  description = "Name of the Key Vault"
  value       = azurerm_key_vault.main.name
}

output "key_vault_uri" {
  description = "URI of the Key Vault"
  value       = azurerm_key_vault.main.vault_uri
}

# Application Insights
output "app_insights_instrumentation_key" {
  description = "Application Insights instrumentation key"
  value       = azurerm_application_insights.main.instrumentation_key
  sensitive   = true
}

output "app_insights_connection_string" {
  description = "Application Insights connection string"
  value       = azurerm_application_insights.main.connection_string
  sensitive   = true
}