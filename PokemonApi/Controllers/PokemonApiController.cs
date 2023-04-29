using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PokemonApi.Data;
using PokemonApi.DataConfiguration;
using PokemonApi.Helpers;
using PokemonApi.Models;
using System.Text;
using System.Text.Json;

namespace PokemonApi.Controllers
{
    [ApiController]
    [Route("api/pokemon")]
    public class PokemonApiController : ControllerBase
    {
        private readonly IPokemonStatsRepository _pokemonStatsRepo;
        private readonly IPokemonImagesRepository _pokemonImagesRepo;
        private readonly IDistributedCache _pokemonCache;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public PokemonApiController(
            IPokemonStatsRepository pokemonStatsRepo,
            IPokemonImagesRepository pokemonImagesRepo,
            IDistributedCache pokemonCache)
        {
            _pokemonStatsRepo = pokemonStatsRepo;
            _pokemonImagesRepo = pokemonImagesRepo;
            _pokemonCache = pokemonCache;

            _cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30));
        }

        [HttpGet("namesNumsImages")]
        public async Task<IActionResult> GetAllNamesNumsImagesAsync()
        {
            var allNamesNumsImages = new List<PokemonNamesNumsImages>();

            var cacheItem = await _pokemonCache.GetAsync("allNamesNumsImages");

            if (cacheItem != null)
            {
                allNamesNumsImages = JsonSerializer.Deserialize<List<PokemonNamesNumsImages>>(Encoding.UTF8.GetString(cacheItem));
                return Ok(allNamesNumsImages);
            }

            var allNamesNumsTask = _pokemonStatsRepo.GetAllNamesAndPokedexNumsAsync();
            var allImagesTask = _pokemonImagesRepo.GetAllImagesAsync();
            await Task.WhenAll(allNamesNumsTask, allImagesTask);

            var allNamesNums = await allNamesNumsTask;
            var allImages = await allImagesTask;

            foreach (var pokemon in allNamesNums)
            {
                var image = allImages.Find(p => p.Name.ToLower() == pokemon.name.ToLower());

                if (image == null) continue;

                allNamesNumsImages.Add(new PokemonNamesNumsImages
                {
                    pokedex_number = pokemon.pokedex_number,
                    name = pokemon.name,
                    FileName = image.FileName,
                    ImageBase64Data = image.ImageBase64Data
                });
            }

            await _pokemonCache.SetAsync("allNamesNumsImages", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(allNamesNumsImages)), _cacheOptions);

            return Ok(allNamesNumsImages);
        }

        [HttpGet("pokedexnumber/{pokedexNum}")]
        public async Task<IActionResult> GetByNumberAsync(int pokedexNum)
        {
            var pokemon = new Pokemon();

            var cacheItem = await _pokemonCache.GetAsync(pokedexNum.ToString());

            if (cacheItem != null)
            {
                pokemon = JsonSerializer.Deserialize<Pokemon>(Encoding.UTF8.GetString(cacheItem));
                return Ok(pokemon);
            }

            var stats = await _pokemonStatsRepo.GetStatsByPokedexNumAsync(pokedexNum);

            if (stats != null && stats.name != null)
            {
                var image = await _pokemonImagesRepo.GetImageByNameAsync(stats.name);
                pokemon = PokemonHelpers.CreatePokemon(stats, image);
                await CachePokemonAsync(pokemon);
                return Ok(pokemon);
            }

            return Ok();
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByNameAsync(string name)
        {
            var pokemon = new Pokemon();

            var cacheItem = await _pokemonCache.GetAsync(name.ToLower());

            if (cacheItem != null)
            {
                pokemon = JsonSerializer.Deserialize<Pokemon>(Encoding.UTF8.GetString(cacheItem));
                return Ok(pokemon);
            }

            var statsTask = _pokemonStatsRepo.GetStatsByNameAsync(name);
            var imageTask = _pokemonImagesRepo.GetImageByNameAsync(name);

            await Task.WhenAll(statsTask, imageTask);

            var stats = await statsTask;
            var image = await imageTask;

            if (stats != null && image != null)
            {
                pokemon = PokemonHelpers.CreatePokemon(stats, image);
                await CachePokemonAsync(pokemon);
                return Ok(pokemon);
            }

            return Ok();
        }

        // This controller method removes any requirement for dynamic SQL statements to be generated while also implementing allow-list to prevent abuse of the API.
        // Another advantage is it allows me to keep all debugging within C# (ie. no need to debug within SQL).
        [HttpGet("filter")]
        public async Task<IActionResult> GetByFilterAsync([FromQuery] IDictionary<string, List<string>> queryParams)
        {
            // C# has some good black magic where it can automatically parse/group the query parameters from the URL if it reads it as a Dictionary object,
            // hence the queryParams argument type of IDictionary<string, List<string>>

            var cleanedQueryParams = ApiHelpers.CleanQueryParameters(queryParams);

            var allStats = new List<List<PokemonStats>>();

            foreach (var kvp in cleanedQueryParams)
            {
                var key = kvp.Key;
                var filterConditions = kvp.Value;

                var stats = new List<PokemonStats>();
                var filterCondition = filterConditions.FirstOrDefault();
                var filterValue = filterConditions.LastOrDefault();

                var cacheItem = await _pokemonCache.GetAsync(key);
                if (cacheItem != null)
                {
                    stats = JsonSerializer.Deserialize<List<PokemonStats>>(Encoding.UTF8.GetString(cacheItem));
                }
                else
                {
                    // The following code executes the spPokemonStats_Get_ByColumn stored procedure in the SQL database, and returns all data
                    // from the three columns: name, pokedex_number, "column_name"
                    // Equivalent SQL statement: SELECT * from name, pokedex_number, "column_name"
                    stats = await _pokemonStatsRepo.GetStatsByAttributeAsync(key);

                    // Cache the SQL query response since this is a relatively expensive operation. Set cache-key-name to be the column name that was retrieved.
                    await _pokemonCache.SetAsync(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(stats)), _cacheOptions);
                }

                if (stats == null || filterCondition == null) continue;

                if (PokemonPropertyDataTypes.integerProperties.Contains(key))
                {
                    switch (filterCondition)
                    {
                        case "EQUALS":
                            var statsFiltered = stats.Where(p => (int)p.GetType().GetProperty(key).GetValue(p) == int.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "GREATER_THAN":
                            statsFiltered = stats.Where(p => (int)p.GetType().GetProperty(key).GetValue(p) > int.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "GREATER_THAN_OR_EQUALS":
                            statsFiltered = stats.Where(p => (int)p.GetType().GetProperty(key).GetValue(p) >= int.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "LESS_THAN":
                            statsFiltered = stats.Where(p => (int)p.GetType().GetProperty(key).GetValue(p) < int.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "LESS_THAN_OR_EQUALS":
                            statsFiltered = stats.Where(p => (int)p.GetType().GetProperty(key).GetValue(p) <= int.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        default:
                            throw new ArgumentException("An invalid filter condition string was attempted.");
                    }
                }
                else if (PokemonPropertyDataTypes.doubleProperties.Contains(key))
                {
                    switch (filterCondition)
                    {
                        case "EQUALS":
                            var statsFiltered = stats.Where(p => (double)p.GetType().GetProperty(key).GetValue(p) == double.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "GREATER_THAN":
                            statsFiltered = stats.Where(p => (double)p.GetType().GetProperty(key).GetValue(p) > double.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "GREATER_THAN_OR_EQUALS":
                            statsFiltered = stats.Where(p => (double)p.GetType().GetProperty(key).GetValue(p) >= double.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "LESS_THAN":
                            statsFiltered = stats.Where(p => (double)p.GetType().GetProperty(key).GetValue(p) < double.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "LESS_THAN_OR_EQUALS":
                            statsFiltered = stats.Where(p => (double)p.GetType().GetProperty(key).GetValue(p) <= double.Parse(filterValue)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        default:
                            throw new ArgumentException("An invalid filter condition string was attempted.");
                    }
                }
                else if (PokemonPropertyDataTypes.stringProperties.Contains(key))
                {
                    switch (filterCondition)
                    {
                        case "EQUALS":
                            var statsFiltered = stats.Where(p => p.GetType().GetProperty(key).GetValue(p).ToString().ToLower().Equals(filterValue.ToLower())).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "CONTAINS":
                            statsFiltered = stats.Where(p => p.GetType().GetProperty(key).GetValue(p).ToString().ToLower().Contains(filterValue.ToLower())).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        default:
                            throw new ArgumentException("An invalid filter condition string was attempted.");
                    }
                }
                else if (PokemonPropertyDataTypes.boolProperties.Contains(key))
                {
                    switch (filterCondition)
                    {
                        case "TRUE":
                            var statsFiltered = stats.Where(p => p.GetType().GetProperty(key).GetValue(p).Equals(true)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        case "FALSE":
                            statsFiltered = stats.Where(p => p.GetType().GetProperty(key).GetValue(p).Equals(false)).ToList();
                            allStats.Add(statsFiltered);
                            break;

                        default:
                            throw new ArgumentException("An invalid filter condition string was attempted.");
                    }
                }
            }

            // Obtain intersection of all queried Pokemon based on equal pokedex_number in each list
            var combinedResult = allStats
                                .Aggregate((previousList, nextList) => previousList
                                .Intersect(nextList, new PokemonStatsComparer())
                                .ToList());

            var combinedResultNamesNums = new List<PokemonNamesNums>();

            foreach (var pokemon in combinedResult)
            {
                combinedResultNamesNums.Add(new PokemonNamesNums
                {
                    name = pokemon.name,
                    pokedex_number = pokemon.pokedex_number,
                });
            }

            return Ok(combinedResultNamesNums);
        }

        private async Task CachePokemonAsync(Pokemon pokemon)
        {
            var cacheByNum = _pokemonCache.SetAsync(pokemon.pokedex_number.ToString(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pokemon)), _cacheOptions);
            var cacheByName = _pokemonCache.SetAsync(pokemon.name.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pokemon)), _cacheOptions);

            await Task.WhenAll(cacheByNum, cacheByName);
        }
    }
}