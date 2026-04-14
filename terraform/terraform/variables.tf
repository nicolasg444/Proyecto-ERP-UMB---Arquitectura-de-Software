# variables.tf - ERP Heladería Biodiversidad
# Variables de configuración - Sprint 2 HU-07

variable "db_admin_user" {
  description = "Usuario administrador de la base de datos Azure SQL"
  type        = string
  default     = "erp_admin"
}

variable "db_admin_password" {
  description = "Contraseña del administrador de la base de datos"
  type        = string
  sensitive   = true
}

variable "location" {
  description = "Región de Azure donde se desplegará la infraestructura"
  type        = string
  default     = "East US"
}

variable "environment" {
  description = "Ambiente de despliegue (dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "project_name" {
  description = "Nombre del proyecto ERP"
  type        = string
  default     = "erp-heladeria-biodiversidad"
}
