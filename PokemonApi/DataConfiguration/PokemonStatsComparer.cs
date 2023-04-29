using PokemonApi.Models;

namespace PokemonApi.DataConfiguration
{
    public class PokemonStatsComparer : IEqualityComparer<PokemonStats>
    {
        public bool Equals(PokemonStats x, PokemonStats y)
        {
            return x.pokedex_number == y.pokedex_number;
        }

        public int GetHashCode(PokemonStats obj)
        {
            return obj.pokedex_number.GetHashCode();
        }
    }
}