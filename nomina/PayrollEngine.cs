// PayrollEngine.cs - ERP Heladería Biodiversidad
// Módulo RR.HH. - Sprint 2 HU-04
// Principio SRP: esta clase SOLO calcula nómina
// Principio DIP: depende de la interfaz IPayrollParameters

using System;

namespace ERP.Heladeria.RRHH
{
    // Interfaz para Inversión de Dependencias (DIP)
    public interface IPayrollParameters
    {
        decimal SalarioBase { get; }
        decimal PorcentajeSaludEmpleado { get; }   // 4%
        decimal PorcentajePensionEmpleado { get; } // 4%
        decimal PorcentajeSaludEmpresa { get; }    // 8.5%
        decimal PorcentajePensionEmpresa { get; }  // 12%
        decimal PorcentajeARL { get; }             // Según riesgo
        int DiasLaborados { get; }
    }

    // Resultado del cálculo de nómina
    public class PayrollResult
    {
        public decimal SalarioBase { get; set; }
        public decimal DeduccionSalud { get; set; }
        public decimal DeduccionPension { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal SalarioNeto { get; set; }
        public decimal AporteSaludEmpresa { get; set; }
        public decimal AportePensionEmpresa { get; set; }
        public decimal AporteARL { get; set; }
        public decimal TotalCostoEmpresa { get; set; }
        public DateTime FechaCálculo { get; set; }
    }

    // Motor de cálculo - Principio SRP
    public class PayrollEngine
    {
        private readonly IPayrollParameters _parameters;

        // Inyección de dependencias - Principio DIP
        public PayrollEngine(IPayrollParameters parameters)
        {
            _parameters = parameters 
                ?? throw new ArgumentNullException(nameof(parameters));
        }

        public PayrollResult CalcularNomina()
        {
            var salarioBase = _parameters.SalarioBase;

            // Deducciones empleado
            var deduccionSalud = 
                salarioBase * _parameters.PorcentajeSaludEmpleado;
            var deduccionPension = 
                salarioBase * _parameters.PorcentajePensionEmpleado;
            var totalDeducciones = deduccionSalud + deduccionPension;
            var salarioNeto = salarioBase - totalDeducciones;

            // Aportes empresa
            var aporteSaludEmpresa = 
                salarioBase * _parameters.PorcentajeSaludEmpresa;
            var aportePensionEmpresa = 
                salarioBase * _parameters.PorcentajePensionEmpresa;
            var aporteARL = 
                salarioBase * _parameters.PorcentajeARL;
            var totalCostoEmpresa = 
                salarioBase + aporteSaludEmpresa + 
                aportePensionEmpresa + aporteARL;

            return new PayrollResult
            {
                SalarioBase         = salarioBase,
                DeduccionSalud      = deduccionSalud,
                DeduccionPension    = deduccionPension,
                TotalDeducciones    = totalDeducciones,
                SalarioNeto         = salarioNeto,
                AporteSaludEmpresa  = aporteSaludEmpresa,
                AportePensionEmpresa = aportePensionEmpresa,
                AporteARL           = aporteARL,
                TotalCostoEmpresa   = totalCostoEmpresa,
                FechaCálculo        = DateTime.Now
            };
        }
    }
}
