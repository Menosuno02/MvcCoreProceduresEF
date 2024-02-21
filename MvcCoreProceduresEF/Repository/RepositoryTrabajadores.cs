using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Data;

#region PROCEDIMIENTOS ALMACENADOS

/*
CREATE OR ALTER PROCEDURE SP_TRABAJADORES_OFICIO
(@OFICIO NVARCHAR(50), @PERSONAS INT OUT,
@MEDIA INT OUT, @SUMA INT OUT)
AS
	SELECT *
	FROM V_TRABAJADORES
	WHERE OFICIO = @OFICIO
	SELECT @PERSONAS = COUNT(IDTRABAJADOR),
	@MEDIA = AVG(SALARIO), @SUMA = SUM(SALARIO)
	FROM V_TRABAJADORES
	WHERE OFICIO = @OFICIO
GO
*/

#endregion

namespace MvcCoreProceduresEF.Repository
{
    public class RepositoryTrabajadores
    {
        private HospitalContext context;

        public RepositoryTrabajadores(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Trabajador>> GetTrabajadoresAsync()
        {
            var consulta = from datos in this.context.Trabajadores
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Trabajadores
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }

        public async Task<TrabajadoresModel> GetTrabajadoresModelAsync
            (string oficio)
        {
            // La única diferencia es que debemos incluir la palabra
            // out en cada parámetro de salida
            // SP_PROCEDURE @param1, @param2 OUT)
            string sql = "SP_TRABAJADORES_OFICIO @OFICIO, " +
                "@PERSONAS OUT, @MEDIA OUT, @SUMA OUT";
            SqlParameter paramOficio = new SqlParameter("@OFICIO", oficio);
            SqlParameter paramPersonas = new SqlParameter("@PERSONAS", -1);
            SqlParameter paramMedia = new SqlParameter("@MEDIA", -1);
            SqlParameter paramSuma = new SqlParameter("@SUMA", -1);
            // Dirección de los parámetros OUT
            paramPersonas.Direction = ParameterDirection.Output;
            paramMedia.Direction = ParameterDirection.Output;
            paramSuma.Direction = ParameterDirection.Output;
            // Ejecutamos la consulta de selección
            var consulta = this.context.Trabajadores.FromSqlRaw(sql, paramOficio,
                paramPersonas, paramMedia, paramSuma);
            // Creamos nuestro model para recuperar los datos
            TrabajadoresModel model = new TrabajadoresModel();
            // Los parámetros se recuperan despues de extraer los
            // datos del select (cuando se cierra el Reader)
            model.Trabajadores = await consulta.ToListAsync();
            model.Personas = int.Parse(paramPersonas.Value.ToString());
            model.MediaSalarial = int.Parse(paramMedia.Value.ToString());
            model.SumaSalarial = int.Parse(paramSuma.Value.ToString());
            return model;

        }
    }
}
