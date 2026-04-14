# main.tf - ERP Heladería Biodiversidad
# Infraestructura Base - Sprint 2 HU-07

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  required_version = ">= 1.3.0"
}

provider "azurerm" {
  features {}
}

# Resource Group
resource "azurerm_resource_group" "erp_rg" {
  name     = "rg-erp-heladeria-biodiversidad"
  location = "East US"
  tags = {
    proyecto   = "ERP-Heladeria-Biodiversidad"
    sprint     = "Sprint2"
    equipo     = "UMB-ArquitecturaSoftware"
  }
}

# Virtual Network (VPC)
resource "azurerm_virtual_network" "erp_vnet" {
  name                = "vnet-erp-heladeria"
  address_space       = ["10.0.0.0/16"]
  location            = azurerm_resource_group.erp_rg.location
  resource_group_name = azurerm_resource_group.erp_rg.name
  tags = {
    sprint = "Sprint2"
  }
}

# Subred Pública (Frontend / API)
resource "azurerm_subnet" "subnet_public" {
  name                 = "subnet-public-erp"
  resource_group_name  = azurerm_resource_group.erp_rg.name
  virtual_network_name = azurerm_virtual_network.erp_vnet.name
  address_prefixes     = ["10.0.1.0/24"]
}

# Subred Privada (Base de Datos)
resource "azurerm_subnet" "subnet_private" {
  name                 = "subnet-private-erp"
  resource_group_name  = azurerm_resource_group.erp_rg.name
  virtual_network_name = azurerm_virtual_network.erp_vnet.name
  address_prefixes     = ["10.0.2.0/24"]
  service_endpoints    = ["Microsoft.Sql"]
}

# Azure SQL Server
resource "azurerm_mssql_server" "erp_sql_server" {
  name                         = "sql-erp-heladeria-biodiversidad"
  resource_group_name          = azurerm_resource_group.erp_rg.name
  location                     = azurerm_resource_group.erp_rg.location
  version                      = "12.0"
  administrator_login          = var.db_admin_user
  administrator_login_password = var.db_admin_password
  tags = {
    sprint = "Sprint2"
  }
}

# Azure SQL Database
resource "azurerm_mssql_database" "erp_database" {
  name      = "db-erp-heladeria"
  server_id = azurerm_mssql_server.erp_sql_server.id
  sku_name  = "Basic"
  tags = {
    sprint = "Sprint2"
  }
}

# Regla de Firewall - Solo subred privada
resource "azurerm_mssql_virtual_network_rule" "erp_vnet_rule" {
  name      = "vnet-rule-erp"
  server_id = azurerm_mssql_server.erp_sql_server.id
  subnet_id = azurerm_subnet.subnet_private.id
}
