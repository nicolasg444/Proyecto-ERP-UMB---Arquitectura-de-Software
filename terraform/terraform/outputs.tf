# outputs.tf - ERP Heladería Biodiversidad
# Variables de salida - Sprint 2 HU-07

output "resource_group_name" {
  description = "Nombre del Resource Group creado"
  value       = azurerm_resource_group.erp_rg.name
}

output "vnet_name" {
  description = "Nombre de la Virtual Network (VPC)"
  value       = azurerm_virtual_network.erp_vnet.name
}

output "vnet_id" {
  description = "ID de la Virtual Network"
  value       = azurerm_virtual_network.erp_vnet.id
}

output "subnet_public_id" {
  description = "ID de la subred pública (Frontend/API)"
  value       = azurerm_subnet.subnet_public.id
}

output "subnet_private_id" {
  description = "ID de la subred privada (Base de Datos)"
  value       = azurerm_subnet.subnet_private.id
}

output "sql_server_name" {
  description = "Nombre del servidor Azure SQL"
  value       = azurerm_mssql_server.erp_sql_server.name
}

output "sql_server_fqdn" {
  description = "Endpoint de conexión al servidor SQL (usado por el backend)"
  value       = azurerm_mssql_server.erp_sql_server.fully_qualified_domain_name
}

output "database_name" {
  description = "Nombre de la base de datos del ERP"
  value       = azurerm_mssql_database.erp_database.name
}
