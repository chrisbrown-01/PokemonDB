using PokemonApi.Models;

namespace PokemonApi.Data
{
    public interface IPokemonStatsRepository
    {
        Task<PokemonStats?> GetStatsByPokedexNumAsync(int pokedexNum);

        Task<PokemonStats?> GetStatsByNameAsync(string name);

        Task<List<PokemonNamesNums>> GetAllNamesAndPokedexNumsAsync();

        Task<List<PokemonStats>?> GetStatsByAttributeAsync(string columnName);
    }
}