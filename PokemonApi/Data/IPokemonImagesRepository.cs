using PokemonApi.Models;

namespace PokemonApi.Data
{
    public interface IPokemonImagesRepository
    {
        Task<List<PokemonImage>> GetAllImagesAsync();

        Task<PokemonImage?> GetImageByNameAsync(string pokemonName);
    }
}