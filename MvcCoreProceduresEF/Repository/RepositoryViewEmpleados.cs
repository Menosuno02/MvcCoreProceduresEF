using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;

namespace MvcCoreProceduresEF.Repository
{
    public class RepositoryViewEmpleados
    {
        HospitalContext context;

        public RepositoryViewEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        //REALIZAMOS LA PETICION A LA VISTA DE FORMA ASINCRONA.
        //TENEMOS UN METODO LLAMADO ToListAsync() DENTRO DE EF
        //QUE NOS DEVUELVE LAS LISTAS de var consulta DE FORMA ASINCRONA
        public async Task<List<ViewEmpleado>> GetEmpleadosAsync()
        {
            var consulta = from datos in this.context.ViewEmpleados
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}
