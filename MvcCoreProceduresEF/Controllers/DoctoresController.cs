﻿using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repository;

namespace MvcCoreProceduresEF.Controllers
{
    public class DoctoresController : Controller
    {
        private RepositoryDoctores repo;

        public DoctoresController(RepositoryDoctores repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            List<Doctor> doctores = this.repo.GetDoctores();
            ViewData["ESPECIALIDADES"] = this.repo.GetEspecialidades();
            return View(doctores);
        }

        [HttpPost]
        public IActionResult Index(string especialidad, int incremento)
        {
            this.repo.IncrementarSalarioEspecialidad(especialidad, incremento);
            List<Doctor> doctores = this.repo.GetDoctoresEspecialidad(especialidad);
            ViewData["ESPECIALIDADES"] = this.repo.GetEspecialidades();
            return View(doctores);
        }
    }
}
