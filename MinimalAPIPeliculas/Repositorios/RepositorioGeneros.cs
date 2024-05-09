using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIPeliculas.Entidades;
using System.Data.Common;

namespace MinimalAPIPeliculas.Repositorios
{
    public class RepositorioGeneros : IRepositorioGeneros
    {
        private readonly string? connectionString;
        public RepositorioGeneros(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<List<Genero>> ObtenerTodos()
        {

            using (var conexion = new SqlConnection(connectionString))
            {

                var generos = await conexion.QueryAsync<Genero>(@"
                                                                SELECT id, Nombre 
                                                                FROM Generos;");

                return generos.ToList();
            
            }
        
        }

        public async Task<Genero?> ObtenerPorId(int id)
        {

            using (var conexion = new SqlConnection(connectionString))
            {

                var genero = await conexion.QueryFirstOrDefaultAsync<Genero>(@"Select * 
                                                                           from Generos
                                                                           Where id = @Id", new { id });

                return genero;
            }

        }

        public async Task<int> CrearGenero(Genero genero)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var id = await conexion.QuerySingleAsync<int>(@"
                                                        INSERT INTO Generos (Nombre)
                                                        VALUES (@Nombre);

                                                        SELECT SCOPE_IDENTITY();", genero);

                genero.Id = id;
                return id;
            }
        }

        public async Task<bool> Existe(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var existe = await conexion.QuerySingleAsync<bool>(@"if EXISTS 
                                                                    (Select 1 
                                                                    from Generos where id = @id)
	                                                                select 1
                                                                    else
	                                                                select 0", new {id});

                return existe;
            }
        }

        public async Task Actualizar(Genero genero)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync(@"update Generos
                                                set Nombre = @Nombre
                                                where id = @id", genero);
            }
        }

        public async Task Eliminar(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync(@"delete from Generos 
                                                where id = @id", new {id});
            }
        }
    }
}
