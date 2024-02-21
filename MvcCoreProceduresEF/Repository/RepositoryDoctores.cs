using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Data;
using System.Data.Common;

#region PROCEDIMIENTOS ALMACENADOS

/*
CREATE OR ALTER PROCEDURE SP_TODOS_DOCTORES
AS
   SELECT *
   FROM DOCTOR
GO
 
CREATE OR ALTER PROCEDURE SP_INCREMENTAR_SALARIO_DOCTOR
(@INCREMENTO INT, @ESPECIALIDAD VARCHAR(40))
AS
	UPDATE DOCTOR
	SET SALARIO = SALARIO + @INCREMENTO
	WHERE ESPECIALIDAD = @ESPECIALIDAD
GO

CREATE OR ALTER PROCEDURE SP_DOCTORES_ESPECIALIDAD
(@ESPECIALIDAD NVARCHAR(50))
AS
	SELECT *
	FROM DOCTOR
	WHERE ESPECIALIDAD = @ESPECIALIDAD
GO

CREATE OR ALTER PROCEDURE SP_ESPECIALIDADES_DOCTORES
AS
	SELECT DISTINCT ESPECIALIDAD
	FROM DOCTOR
GO
*/

#endregion

namespace MvcCoreProceduresEF.Repository
{
    public class RepositoryDoctores
    {
        private HospitalContext context;

        public RepositoryDoctores(HospitalContext context)
        {
            this.context = context;
        }

        public List<Doctor> GetDoctores()
        {
            string sql = "SP_TODOS_DOCTORES";
            var consulta = this.context.Doctores.FromSqlRaw(sql);
            List<Doctor> doctores = consulta.AsEnumerable().ToList();
            return doctores;
        }

        public List<string> GetEspecialidades()
        {
            using (DbCommand com =
                this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_ESPECIALIDADES_DOCTORES";
                com.CommandText = sql;
                com.CommandType = CommandType.StoredProcedure;
                com.Connection.Open();
                DbDataReader reader = com.ExecuteReader();
                List<string> especialidades = new List<string>();
                while (reader.Read())
                {
                    especialidades.Add(reader["ESPECIALIDAD"].ToString());
                }
                reader.Close();
                com.Connection.Close();
                return especialidades;
            }
        }

        public List<Doctor> GetDoctoresEspecialidad(string especialidad)
        {
            string sql = "SP_DOCTORES_ESPECIALIDAD @ESPECIALIDAD";
            SqlParameter paramEspecialidad = new SqlParameter("@ESPECIALIDAD", especialidad);
            var consulta = this.context.Doctores.FromSqlRaw(sql, paramEspecialidad);
            List<Doctor> doctores = consulta.AsEnumerable().ToList();
            return doctores;
        }

        public void IncrementarSalarioEspecialidad(string especialidad, int incremento)
        {
            string sql = "SP_INCREMENTAR_SALARIO_DOCTOR @INCREMENTO, @ESPECIALIDAD";
            SqlParameter paramEspecialidad = new SqlParameter("@ESPECIALIDAD", especialidad);
            SqlParameter paramIncremento = new SqlParameter("@INCREMENTO", incremento);
            this.context.Database.ExecuteSqlRaw(sql, paramIncremento, paramEspecialidad);
        }
    }
}
