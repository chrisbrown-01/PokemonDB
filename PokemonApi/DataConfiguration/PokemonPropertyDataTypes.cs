namespace PokemonApi.DataConfiguration
{
    public class PokemonPropertyDataTypes
    {
        public static readonly List<string> integerProperties = new List<string>
            {
                "pokedex_number",
                "base_egg_steps",
                "base_happiness",
                "base_total",
                "capture_rate",
                "defense",
                "experience_growth",
                "hp",
                "sp_attack",
                "sp_defense",
                "speed",
                "generation",
                "attack"
            };

        public static readonly List<string> doubleProperties = new List<string>
            {
                "against_bug",
                "against_dark",
                "against_dragon",
                "against_electric",
                "against_fairy",
                "against_fight",
                "against_fire",
                "against_flying",
                "against_ghost",
                "against_grass",
                "against_ground",
                "against_ice",
                "against_normal",
                "against_poison",
                "against_psychic",
                "against_rock",
                "against_steel",
                "against_water",
                "height_m",
                "percentage_male",
                "weight_kg"
            };

        public static readonly List<string> stringProperties = new List<string>
            {
                "name",
                "classification",
                "type1",
                "type2",
            };

        public static readonly List<string> boolProperties = new List<string>
            {
                "is_legendary"
            };
    }
}