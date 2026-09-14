using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Repository
{
    public interface IRepositoryCuentas
    {
        Task Crear(Cuenta cuenta);
    }

    public class RepositorioCuentas : IRepositoryCuentas
    {
        private readonly string _connectionString;

        public RepositorioCuentas(IConfiguration configuration) 
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }


        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(_connectionString);

            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas 
            (Nombre, TipoCuentaId, Descripcion, Balance) VALUES 
                (@Nombre, @TipoCuentaId, @Descripcion, @Balance);

               SELECT SCOPE_IDENTITY(); ", cuenta);

            cuenta.Id = id;
        }
    }
}
