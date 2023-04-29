using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace PokemonApi.DbAccess
{
    public class PokemonStatsSqlDbAccess : IPokemonStatsDbAccess
    {
        private readonly IConfiguration _config;

        public PokemonStatsSqlDbAccess(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionId = "PokemonStatsDatabase")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}