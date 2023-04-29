namespace PokemonApi.Models
{
    public class PokemonNamesNumsImages
    {
        public int pokedex_number { get; set; }
        public string name { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public string? ImageBase64Data { get; set; }
    }
}