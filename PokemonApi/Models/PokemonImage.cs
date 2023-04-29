using MongoDB.Bson;

namespace PokemonApi.Models
{
    public class PokemonImage
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ImageBase64Data { get; set; } = string.Empty;
    }
}