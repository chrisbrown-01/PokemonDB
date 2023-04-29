using PokemonApi.DbAccess;
using PokemonApi.Models;

namespace PokemonApi.Data
{
    public class PokemonStatsSqlRepo : IPokemonStatsRepository
    {
        private readonly IPokemonStatsDbAccess _db;

        public PokemonStatsSqlRepo(IPokemonStatsDbAccess db)
        {
            _db = db;
        }

        public async Task<List<PokemonNamesNums>> GetAllNamesAndPokedexNumsAsync()
        {
            var data = await _db.LoadDataAsync<PokemonNamesNums, dynamic>("dbo.spPokemonStats_GetAll_NamesAndPokedexNums", new { });
            return data.ToList();
        }

        public async Task<PokemonStats?> GetStatsByPokedexNumAsync(int pokedexNum)
        {
            var data = await _db.LoadDataAsync<PokemonStats, dynamic>("dbo.spPokemonStats_Get_ByPokedexNum", new { PokedexNum = pokedexNum });
            return data.FirstOrDefault();
        }

        public async Task<PokemonStats?> GetStatsByNameAsync(string name)
        {
            var data = await _db.LoadDataAsync<PokemonStats, dynamic>("dbo.spPokemonStats_Get_ByName", new { Name = name });
            return data.FirstOrDefault();
        }

        public async Task<List<PokemonStats>?> GetStatsByAttributeAsync(string attribute)
        {
            var data = await _db.LoadDataAsync<PokemonStats, dynamic>("dbo.spPokemonStats_Get_ByColumn", new { ColumnName = attribute });
            return data.ToList();
        }
    }
}