namespace PokemonApi.DbAccess
{
    public interface IPokemonStatsDbAccess
    {
        Task<IEnumerable<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionId = "PokemonStatsDatabase");
    }
}