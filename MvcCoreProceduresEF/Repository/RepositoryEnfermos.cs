using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Data;
using System.Data.Common;

#region PROCEDIMIENTOS ALMACENADOS

/*
CREATE OR ALTER PROCEDURE SP_TODOS_ENFERMOS
AS
	SELECT INSCRIPCION, APELLIDO, DIRECCION, FECHA_NAC,
	CASE S
		WHEN 'M' THEN 'MASCULINO'
		WHEN 'F' THEN 'FEMENINO'
	END AS S
	FROM ENFERMO
GO

CREATE OR ALTER PROCEDURE SP_FIND_ENFERMO
(@INSCRIPCION INT)
AS
	SELECT *
	FROM ENFERMO
	WHERE INSCRIPCION = @INSCRIPCION
GO

CREATE OR ALTER PROCEDURE SP_DELETE_ENFERMO
(@INSCRIPCION INT)
AS
	DELETE FROM ENFERMO
	WHERE INSCRIPCION = @INSCRIPCION
GO

CREATE OR ALTER PROCEDURE SP_CREATE_ENFERMO
(@APELLIDO NVARCHAR(50), @DIRECCION NVARCHAR(50),
@FECHA_NAC DATETIME, @SEXO NVARCHAR(50))
AS
	DECLARE @INSCRIPCION INT
	SELECT @INSCRIPCION = MAX(INSCRIPCION) + 1
	FROM ENFERMO
	INSERT INTO ENFERMO VALUES
	(@INSCRIPCION, @APELLIDO, @DIRECCION, @FECHA_NAC, @SEXO, NULL)
GO
*/

#endregion

namespace MvcCoreProceduresEF.Repository
{
    public class RepositoryEnfermos
    {
        private EnfermoContext context;

        public RepositoryEnfermos(EnfermoContext context)
        {
            this.context = context;
        }

        public List<Enfermo> GetEnfermos()
        {
            // Para consultas de selección con procedimientos
            // almacenados, debemos mapear manualmente los datos
            using (DbCommand com =
                this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_TODOS_ENFERMOS";
                com.CommandText = sql;
                com.CommandType = CommandType.StoredProcedure;
                com.Connection.Open();
                DbDataReader reader = com.ExecuteReader();
                List<Enfermo> enfermos = new List<Enfermo>();
                while (reader.Read())
                {
                    Enfermo enfermo = new Enfermo
                    {
                        Apellido = reader["APELLIDO"].ToString(),
                        Inscripcion = int.Parse(reader["INSCRIPCION"].ToString()),
                        Direccion = reader["DIRECCION"].ToString(),
                        FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString()),
                        Sexo = reader["S"].ToString(),
                    };
                    enfermos.Add(enfermo);
                }
                reader.Close();
                com.Connection.Close();
                return enfermos;
            }
        }

        public Enfermo FindEnfermo(int inscripcion)
        {
            // Para llamar a procedimientos con parámetros la llamada
            // se realiza incluyendo los parámetros y el nombre del procedure
            // SP_NOMBREPROCEDURE @param1, @param2
            string sql = "SP_FIND_ENFERMO @INSCRIPCION";
            // Para declarar parámetros se utiliza la clase SqlParameter
            // Debemos tener cuidado con el namespace
            // Namespace = Microsoft.Data.SqlClient
            SqlParameter paramInscripcion =
                new SqlParameter("@INSCRIPCION", inscripcion);
            // Al ser un procedimiento SELECT, puedo utilizar
            // el método FromSqlRaw para extraer los datos
            // Si mi consulta coincide con un model, puedo utilizar
            // LINQ para mapear los datos
            // Cuando tenemos un procedure select, las peticiones se dividen
            // en dos, no puedo hacer LINQ y después un foreach
            // Debemos extraer los datos en dos acciones
            var consulta = this.context.Enfermos.FromSqlRaw(sql, paramInscripcion);
            // Extraer las entidades de la consulta (ejecutar)
            // Para ejecutar necesitamos AsEnumerable()
            // List<Enfermo> enfermos = consulta.AsEnumerable().ToList();
            Enfermo enfermo = consulta.AsEnumerable().FirstOrDefault();
            return enfermo;
        }

        public void DeleteEnfermo(int inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO @INSCRIPCION";
            SqlParameter paramInscripcion =
                new SqlParameter("@INSCRIPCION", inscripcion);
            // Ejecutar consultas de accion se realiza mediante el método
            // ExecuteSqlRaw que se accede desde el Database dentro del DbContext
            this.context.Database.ExecuteSqlRaw(sql, paramInscripcion);
        }

        public void InsertEnfermo(string apellido, string direccion, DateTime fechanacimiento, string sexo)
        {
            string sql = "SP_CREATE_ENFERMO @APELLIDO, @DIRECCION, @FECHA_NAC, @SEXO";
            SqlParameter paramApellido = new SqlParameter("@APELLIDO", apellido);
            SqlParameter paramDireccion = new SqlParameter("@DIRECCION", direccion);
            SqlParameter paramFechaNac = new SqlParameter("@FECHA_NAC", fechanacimiento);
            SqlParameter paramSexo = new SqlParameter("@SEXO", sexo);
            this.context.Database.ExecuteSqlRaw(sql, paramApellido, paramDireccion,
                paramFechaNac, paramSexo);
        }
    }
}
