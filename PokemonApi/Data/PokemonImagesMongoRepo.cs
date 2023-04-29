using MongoDB.Driver;
using PokemonApi.Models;

namespace PokemonApi.Data
{
    public class PokemonImagesMongoRepo : IPokemonImagesRepository
    {
        private readonly IConfiguration _config;
        private MongoClient MongoClient { get; set; }
        private IMongoDatabase MongoDatabase { get; set; }

        public PokemonImagesMongoRepo(IConfiguration config)
        {
            _config = config;
            MongoDatabase = ConnectToMongoDatabase();
        }

        private IMongoDatabase ConnectToMongoDatabase(string connectionId = "PokemonImagesDatabase", string databaseName = "PokemonImagesDb")
        {
            MongoClient = new MongoClient(_config.GetConnectionString(connectionId));
            return MongoClient.GetDatabase(databaseName);
        }

        //
        public async Task<List<PokemonImage>> GetAllImagesAsync()
        {
            var imagesCollection = MongoDatabase.GetCollection<PokemonImage>("PokemonImages");
            var allImages = await imagesCollection.FindAsync(_ => true);

            return await allImages.ToListAsync();
        }

        public async Task<PokemonImage?> GetImageByNameAsync(string pokemonName)
        {
            var imagesCollection = MongoDatabase.GetCollection<PokemonImage>("PokemonImages");
            var image = await imagesCollection.FindAsync(p => p.Name.ToLower() == pokemonName.ToLower());

            return image.FirstOrDefault();
        }
    }
}