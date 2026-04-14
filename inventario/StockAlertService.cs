// StockAlertService.cs - ERP Heladería Biodiversidad
// Módulo Inventario - Sprint 2 HU-05
// Alerta automática cuando stock baja del umbral mínimo

using System;
using System.Collections.Generic;

namespace ERP.Heladeria.Inventario
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public string UnidadMedida { get; set; }
    }

    public class AlertaStock
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public int UnidadesFaltantes { get; set; }
        public string Nivel { get; set; } // "CRITICO" o "BAJO"
        public DateTime FechaAlerta { get; set; }
    }

    public class StockAlertService
    {
        private readonly List<Producto> _productos;

        public StockAlertService(List<Producto> productos)
        {
            _productos = productos 
                ?? throw new ArgumentNullException(nameof(productos));
        }

        public List<AlertaStock> VerificarStockCritico()
        {
            var alertas = new List<AlertaStock>();

            foreach (var producto in _productos)
            {
                if (producto.StockActual <= producto.StockMinimo)
                {
                    alertas.Add(new AlertaStock
                    {
                        ProductoId       = producto.Id,
                        NombreProducto   = producto.Nombre,
                        StockActual      = producto.StockActual,
                        StockMinimo      = producto.StockMinimo,
                        UnidadesFaltantes = producto.StockMinimo 
                                           - producto.StockActual,
                        Nivel            = producto.StockActual == 0 
                                           ? "CRITICO" : "BAJO",
                        FechaAlerta      = DateTime.Now
                    });
                }
            }

            return alertas;
        }

        public bool ProductoEnStockCritico(int productoId)
        {
            var producto = _productos.Find(p => p.Id == productoId);
            if (producto == null) return false;
            return producto.StockActual <= producto.StockMinimo;
        }
    }
}
