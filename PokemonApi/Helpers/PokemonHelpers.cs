using PokemonApi.Models;

namespace PokemonApi.Helpers
{
    public class PokemonHelpers
    {
        public static Pokemon CreatePokemon(PokemonStats stats, PokemonImage image)
        {
            var pokemon = new Pokemon
            {
                pokedex_number = stats.pokedex_number,
                name = stats.name,
                against_bug = stats.against_bug,
                against_dark = stats.against_dark,
                against_dragon = stats.against_dragon,
                against_electric = stats.against_electric,
                against_fairy = stats.against_fairy,
                against_fight = stats.against_fight,
                against_fire = stats.against_fire,
                against_flying = stats.against_flying,
                against_ghost = stats.against_ghost,
                against_grass = stats.against_grass,
                against_ground = stats.against_ground,
                against_ice = stats.against_ice,
                against_normal = stats.against_normal,
                against_poison = stats.against_poison,
                against_psychic = stats.against_psychic,
                against_rock = stats.against_rock,
                against_steel = stats.against_steel,
                against_water = stats.against_water,
                attack = stats.attack,
                base_egg_steps = stats.base_egg_steps,
                base_total = stats.base_total,
                capture_rate = stats.capture_rate,
                classification = stats.classification,
                defense = stats.defense,
                experience_growth = stats.experience_growth,
                height_m = stats.height_m,
                hp = stats.hp,
                percentage_male = stats.percentage_male,
                sp_attack = stats.sp_attack,
                sp_defense = stats.sp_defense,
                speed = stats.speed,
                type1 = stats.type1,
                type2 = stats.type2,
                weight_kg = stats.weight_kg,
                generation = stats.generation,
                is_legendary = stats.is_legendary,
                FileName = image.FileName,
                ImageBase64Data = image.ImageBase64Data
            };

            return pokemon;
        }
    }
}