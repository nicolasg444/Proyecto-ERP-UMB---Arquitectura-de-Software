# arc42 - Documentación de Arquitectura
## ERP Heladería Biodiversidad - Sprint 2

---

## Sección 5 — Building Block View (Vista de Bloques)

### Microservicio de Nómina (HU-04)

El módulo de RR.HH. sigue el principio de 
Responsabilidad Única (SRP): la clase `PayrollEngine` 
solo calcula nómina. La Inversión de Dependencias (DIP) 
se aplica mediante la interfaz `IPayrollParameters`, 
permitiendo cambiar los parámetros sin modificar 
el motor de cálculo.


---

## Sección 7 — Deployment View (Vista de Despliegue)

### Infraestructura Base con Terraform (HU-07)

La infraestructura del ERP se provisiona mediante 
código Terraform versionado en GitHub. Se despliega 
en Microsoft Azure con la siguiente estructura:


**Outputs disponibles para el backend:**
- `sql_server_fqdn` → Endpoint de conexión a la BD
- `subnet_public_id` → ID subred para el API
- `subnet_private_id` → ID subred para la BD

---

## Sección 10 — Architecture Concepts (Seguridad)

### Modelo de Control de Acceso Basado en Roles - RBAC (HU-06)

El sistema implementa RBAC con los siguientes roles:

| Rol | Compras | RR.HH. | Inventario | Facturación |
|---|---|---|---|---|
| Admin | ✅ Total | ✅ Total | ✅ Total | ✅ Total |
| Compras | ✅ Ver/Crear/Editar | ❌ | ✅ Ver | ❌ |
| RR.HH. | ❌ | ✅ Ver/Crear/Editar | ❌ | ❌ |
| Inventario | ❌ | ❌ | ✅ Ver/Crear/Editar | ❌ |
| Facturación | ❌ | ❌ | ✅ Ver | ✅ Ver/Crear/Editar |
| Cocina | ❌ | ❌ | ✅ Ver | ❌ |

### Nueva cláusula DoD - Sprint 2
> ✅ El código de IaC (Terraform) ha sido revisado,
> aprobado mediante Pull Request, aplicado en el 
> entorno de desarrollo y versionado en GitHub.

---

## Decisiones de Arquitectura - Sprint 2

| ID | Decisión | Justificación |
|---|---|---|
| DA-01 | Azure como proveedor cloud | Stack tecnológico del equipo usa Azure |
| DA-02 | Principio SRP en PayrollEngine | Facilita pruebas unitarias y mantenimiento |
| DA-03 | RBAC para control de acceso | Requisito no funcional de seguridad definido en Sprint 0 |
| DA-04 | Terraform para IaC | Naturaleza declarativa, idempotente y versionable |
