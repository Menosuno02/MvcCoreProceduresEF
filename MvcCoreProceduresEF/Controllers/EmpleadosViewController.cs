using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repository;

namespace MvcCoreProceduresEF.Controllers
{
    public class EmpleadosViewController : Controller
    {
        private RepositoryViewEmpleados repo;

        public EmpleadosViewController(RepositoryViewEmpleados repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<ViewEmpleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
    }
}
