// RoleService.cs - ERP Heladería Biodiversidad
// Módulo Seguridad - Sprint 2 HU-06
// Control de acceso basado en roles (RBAC)

using System;
using System.Collections.Generic;

namespace ERP.Heladeria.Seguridad
{
    public enum RolUsuario
    {
        Admin,
        Compras,
        RRHH,
        Inventario,
        Facturacion,
        Cocina
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public RolUsuario Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaAsignacionRol { get; set; }
    }

    public class PermisoModulo
    {
        public string Modulo { get; set; }
        public bool PuedeVer { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
    }

    public class RoleService
    {
        // Mapa de permisos por rol (RBAC)
        private static readonly Dictionary<RolUsuario, 
            List<PermisoModulo>> _permisos = 
            new Dictionary<RolUsuario, List<PermisoModulo>>
        {
            {
                RolUsuario.Admin, new List<PermisoModulo>
                {
                    new PermisoModulo { Modulo = "Compras",
                        PuedeVer=true, PuedeCrear=true,
                        PuedeEditar=true, PuedeEliminar=true },
                    new PermisoModulo { Modulo = "RRHH",
                        PuedeVer=true, PuedeCrear=true,
                        PuedeEditar=true, PuedeEliminar=true },
                    new PermisoModulo { Modulo = "Inventario",
                        PuedeVer=true, PuedeCrear=true,
                        PuedeEditar=true, PuedeEliminar=true },
                    new PermisoModulo { Modulo = "Facturacion",
                        PuedeVer=true, PuedeCrear=true,
                        PuedeEditar=true, PuedeEliminar=true }
                }
            },
            {
                RolUsuario.Compras, new List<PermisoModulo>
                {
                    new PermisoModulo { Modulo = "Compras",
                        PuedeVer=true, PuedeCrear=true,
                        PuedeEditar=true, PuedeEliminar=false },
                    new PermisoModulo { Modulo = "Inventario",
                        PuedeVer=true, PuedeCrear=false,
                        PuedeEditar=false, PuedeEliminar=false }
                }
            },
            {
                RolUsuario.RRHH, new List<PermisoModulo>
                {
                    new PermisoModulo { Modulo = "RRHH",
                        PuedeVer=true, PuedeCrear=true,
                        PuedeEditar=true, PuedeEliminar=false }
                }
            },
            {
                RolUsuario.Inventario, new List<PermisoModulo>
                {
                    new PermisoModulo { Modulo = "Inventario",
                        PuedeVer=true, PuedeCrear=true,
                        PuedeEditar=true, PuedeEliminar=false }
                }
            }
        };

        public Usuario AsignarRol(Usuario usuario, RolUsuario nuevoRol)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            usuario.Rol = nuevoRol;
            usuario.FechaAsignacionRol = DateTime.Now;
            return usuario;
        }

        public bool TienePermiso(Usuario usuario, 
            string modulo, string accion)
        {
            if (!usuario.Activo) return false;
            if (!_permisos.ContainsKey(usuario.Rol)) return false;

            var permisosRol = _permisos[usuario.Rol];
            var permiso = permisosRol.Find(p => p.Modulo == modulo);
            if (permiso == null) return false;

            return accion switch
            {
                "ver"      => permiso.PuedeVer,
                "crear"    => permiso.PuedeCrear,
                "editar"   => permiso.PuedeEditar,
                "eliminar" => permiso.PuedeEliminar,
                _          => false
            };
        }

        public List<PermisoModulo> ObtenerPermisos(RolUsuario rol)
        {
            return _permisos.ContainsKey(rol) 
                ? _permisos[rol] 
                : new List<PermisoModulo>();
        }
    }
}
