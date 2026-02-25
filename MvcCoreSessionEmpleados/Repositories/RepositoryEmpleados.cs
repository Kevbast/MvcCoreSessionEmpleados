using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MvcCoreSessionEmpleados.Data;
using MvcCoreSessionEmpleados.Models;

namespace MvcCoreSessionEmpleados.Repositories
{
    public class RepositoryEmpleados
    {
        private HospitalContext context;

        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            var consulta = from datos in this.context.Empleados
                           select datos;

                return await consulta.ToListAsync();
        }

        public async Task<Empleado> FindEmpleadoAsync(int idEmpleado)
        {
            var consulta = from datos in this.context.Empleados
                           where datos.IdEmpleado==idEmpleado
                           select datos;

            return await consulta.FirstOrDefaultAsync();
        }

        //Vamos a crear un método nuevo que reciba una coleccion de los ids de los empleados y devolverá una lista de empleados

        public async Task<List<Empleado>> GetEmpleadosPorColeccion(List<int> idsEmpleados)
        {

            var consulta = from datos in this.context.Empleados
                           where idsEmpleados.Contains(datos.IdEmpleado)
                           select datos;

            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosSinRepetirAsync(List<int>? idsEmpleados)
        {
            var consulta = from datos in this.context.Empleados
                           where !(idsEmpleados.Contains(datos.IdEmpleado))
                           select datos;

            return await consulta.ToListAsync();
        }


    }
}
