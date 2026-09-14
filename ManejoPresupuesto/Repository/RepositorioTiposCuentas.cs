using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Repository
{
    
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);

        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    { 
        private readonly string _connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear (TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);

            var id = await connection.QuerySingleAsync<int>
                ("TiposCuentasInsertar", new
                {
                    usuarioId = tipoCuenta.UsuarioId,
                    nombre = tipoCuenta.Nombre
                }, commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.Id = id;
        }


        public async Task<bool> Existe(string nombre, int usuarioId) {

            using var connection = new SqlConnection(_connectionString);

            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"Select " +
                $"1 from TiposCuentas where Nom" +
                $"bre = @Nombre and UsuarioId = @UsuarioId;",
                new {nombre, usuarioId});


            return existe == 1;


        }


        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id
              ,Nombre
              ,UsuarioId
              ,Orden
                FROM TiposCuentas where UsuarioId = @UsuarioId
                ORDER BY Orden;", new {usuarioId});
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(@"update TiposCuentas 
                    set Nombre = @Nombre where Id = @Id;", tipoCuenta); //Ejecutar un query que no retorna nada


        }


        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>
                (@"SELECT
                    Id, Nombre, Orden 
                    FROM TiposCuentas where Id = @Id and UsuarioId = @UsuarioId;", new {id, usuarioId});
        }


        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection (_connectionString);

            await connection.ExecuteAsync("DELETE TiposCuentas WHERE Id = @Id;", new {id});


        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados)
        {
            var query = @"UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id;";

            var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(query, tiposCuentasOrdenados);
        }
    }
}
