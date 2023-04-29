using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using PokemonApi.Controllers;
using PokemonApi.Data;
using PokemonApi.Models;
using PokemonApi_Tests.TestData;
using System.Text;
using System.Text.Json;
using System.Web;

namespace PokemonApi_Tests
{
    // NOTE: Exception handling (ie. checking for HTTP 500 error codes) cannot be testing in unit tests because of the global middleware exception handling.
    public class PokemonApiController_UnitTests
    {
        private PokemonApiController _controller;
        private Fixture _fixture;
        private Mock<IPokemonStatsRepository> _pokemonStatsRepo;
        private Mock<IPokemonImagesRepository> _pokemonImagesRepo;
        private Mock<IDistributedCache> _pokemonCache;

        public static IEnumerable<object[]> ValidFilterQueries =>
        new List<object[]>
            {
                // https://localhost:7274/api/pokemon/filter?

                // Integer properties:
                new object[] { "pokedex_number=EQUALS&pokedex_number=1" },
                new object[] { "base_egg_steps=EQUALS&base_egg_steps=1" },
                new object[] { "base_happiness=EQUALS&base_happiness=1" },
                new object[] { "base_total=EQUALS&base_total=1" },
                new object[] { "capture_rate=EQUALS&capture_rate=1" },
                new object[] { "defense=EQUALS&defense=1" },
                new object[] { "experience_growth=EQUALS&experience_growth=1" },
                new object[] { "hp=EQUALS&hp=1"  },
                new object[] { "sp_attack=EQUALS&sp_attack=1" },
                new object[] { "sp_defense=EQUALS&sp_defense=1" },
                new object[] { "speed=EQUALS&speed=1" },
                new object[] { "generation=EQUALS&generation=1" },
                new object[] { "attack=EQUALS&attack=1" },
                new object[] { "pokedex_number=GREATER_THAN&pokedex_number=1" },
                new object[] { "pokedex_number=GREATER_THAN_OR_EQUALS&pokedex_number=1" },
                new object[] { "pokedex_number=LESS_THAN&pokedex_number=1" },
                new object[] { "pokedex_number=LESS_THAN_OR_EQUALS&pokedex_number=1" },

                // Double properties:
                new object[] { "against_bug=EQUALS&against_bug=0.55" },
                new object[] { "against_dark=EQUALS&against_dark=0.55" },
                new object[] { "against_dragon=EQUALS&against_dragon=0.55" },
                new object[] { "against_electric=EQUALS&against_electric=0.55" },
                new object[] { "against_fairy=EQUALS&against_fairy=0.55" },
                new object[] { "against_fight=EQUALS&against_fight=0.55" },
                new object[] { "against_fire=EQUALS&against_fire=0.55" },
                new object[] { "against_flying=EQUALS&against_flying=0.55" },
                new object[] { "against_ghost=EQUALS&against_ghost=0.55" },
                new object[] { "against_grass=EQUALS&against_grass=0.55" },
                new object[] { "against_ground=EQUALS&against_ground=0.55" },
                new object[] { "against_ice=EQUALS&against_ice=0.55" },
                new object[] { "against_normal=EQUALS&against_normal=0.55" },
                new object[] { "against_poison=EQUALS&against_poison=0.55" },
                new object[] { "against_psychic=EQUALS&against_psychic=0.55" },
                new object[] { "against_rock=EQUALS&against_rock=0.55" },
                new object[] { "against_steel=EQUALS&against_steel=0.55" },
                new object[] { "against_water=EQUALS&against_water=0.55" },
                new object[] { "height_m=EQUALS&height_m=0.55" },
                new object[] { "percentage_male=EQUALS&percentage_male=0.55" },
                new object[] { "weight_kg=EQUALS&weight_kg=0.55" },
                new object[] { "against_bug=GREATER_THAN&against_bug=0.1" },
                new object[] { "against_bug=GREATER_THAN_OR_EQUALS&against_bug=1.1" },
                new object[] { "against_bug=LESS_THAN&against_bug=2.1" },
                new object[] { "against_bug=LESS_THAN_OR_EQUALS&against_bug=3.1" },

                // String properties:
                new object[] { "name=EQUALS&name=abc" },
                new object[] { "classification=EQUALS&classification=abc" },
                new object[] { "type1=EQUALS&type1=abc" },
                new object[] { "type2=EQUALS&type2=abc" },
                new object[] { "name=CONTAINS&name=abc" },

                // Bool properties:
                new object[] { "is_legendary=TRUE&is_legendary=" },
                new object[] { "is_legendary=FALSE&is_legendary=" },
                new object[] { "is_legendary=FALSE&is_legendary=anystring123" },
                new object[] { "is_legendary=FALSE" },
                new object[] { "is_legendary=TRUE" },

                // Mixed properties:
                new object[] { "name=CONTAINS&name=abcd&pokedex_number=GREATER_THAN&pokedex_number=10&against_dark=LESS_THAN_OR_EQUALS&against_dark=2.5&is_legendary=TRUE&is_legendary=" },

                // Duplicated properties:
                new object[] { "name=CONTAINS&name=abcd&name=CONTAINS&name=efgh" },

                // Mixed and duplicated properties:
                new object[] { "name=CONTAINS&name=abcd&name=CONTAINS&name=efgh&pokedex_number=GREATER_THAN&pokedex_number=10&against_dark=LESS_THAN_OR_EQUALS&against_dark=2.5" }
            };

        public static IEnumerable<object[]> InvalidFilterQueries =>
        new List<object[]>
        {
                // https://localhost:7274/api/pokemon/filter?

                new object[] { "non_existent_key=EQUALS&non_existent_key=1" }, // Non-existing property passed into key field

                new object[] { "pokedex_number=GREATER_THAN&name=1" }, // Mixing of existing properties with incomplete query fields

                new object[] { "pokedex_number=EQUALS&pokedex_number=1.1" }, // Non-int passed into int field
                new object[] { "against_bug=EQUALS&against_bug=not_a_double" }, // Non-double passed into double field

                new object[] { "pokedex_number=EQUALS" }, // Incomplete int parameter query
                new object[] { "against_bug=EQUALS" }, // Incomplete double parameter query

                new object[] { "pokedex_number=NOT_A_FILTER_CONDITION&pokedex_number=1" }, // Non-existing filter condition
                new object[] { "against_bug=NOT_A_FILTER_CONDITION&against_bug=1.1" }, // Non-existing filter condition
                new object[] { "name=NOT_A_FILTER_CONDITION&name=abc" }, // Non-existing filter condition
                new object[] { "is_legendary=NOT_A_FILTER_CONDITION&is_legendary=" }, // Non-existing filter condition
        };

        public PokemonApiController_UnitTests()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new ObjectIdGenerator());

            _pokemonStatsRepo = new Mock<IPokemonStatsRepository>();
            _pokemonImagesRepo = new Mock<IPokemonImagesRepository>();
            _pokemonCache = new Mock<IDistributedCache>();

            _controller = new PokemonApiController(_pokemonStatsRepo.Object, _pokemonImagesRepo.Object, _pokemonCache.Object);
        }

        [Fact]
        public async Task GetByNumberAsync_Uncached_Returns_Ok()
        {
            var pokemonStats = _fixture.Create<PokemonStats>();
            _pokemonStatsRepo.Setup(repo => repo.GetStatsByPokedexNumAsync(It.IsAny<int>())).ReturnsAsync(pokemonStats);

            var pokemonImage = _fixture.Create<PokemonImage>();
            _pokemonImagesRepo.Setup(repo => repo.GetImageByNameAsync(It.IsAny<string>())).ReturnsAsync(pokemonImage);

            byte[]? cacheItem = null;
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetByNumberAsync(It.IsAny<int>());
            var obj = result as ObjectResult;

            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetByNumberAsync_Cached_Returns_Ok()
        {
            var pokemon = _fixture.Create<Pokemon>();
            var cacheItem = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pokemon));
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetByNumberAsync(It.IsAny<int>());
            var obj = result as ObjectResult;

            obj.StatusCode.Should().Be(200);
        }

        // Test to ensure that if a valid integer (ie. a value that exists in the Pokemon repos) is entered, the
        // response body is not empty.
        [Fact]
        public async Task GetByNumberAsync_ExistingInputNumber_Returns_NonEmptyResult()
        {
            var pokemonStats = _fixture.Create<PokemonStats>();
            _pokemonStatsRepo.Setup(repo => repo.GetStatsByPokedexNumAsync(It.IsAny<int>())).ReturnsAsync(pokemonStats);

            var pokemonImage = _fixture.Create<PokemonImage>();
            _pokemonImagesRepo.Setup(repo => repo.GetImageByNameAsync(It.IsAny<string>())).ReturnsAsync(pokemonImage);

            byte[]? cacheItem = null;
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetByNumberAsync(It.IsAny<int>());
            var obj = result as ObjectResult;

            obj.Should().NotBeNull();
        }

        // Test to ensure that if a non-valid integer (ie. a value that does not exist in the Pokemon repos) is entered, the
        // response body is empty.
        [Fact]
        public async Task GetByNumberAsync_NonExistingInputNumber_Returns_EmptyResult()
        {
            PokemonStats? pokemonStats = null;
            _pokemonStatsRepo.Setup(repo => repo.GetStatsByPokedexNumAsync(It.IsAny<int>())).ReturnsAsync(pokemonStats);

            PokemonImage? pokemonImage = null;
            _pokemonImagesRepo.Setup(repo => repo.GetImageByNameAsync(It.IsAny<string>())).ReturnsAsync(pokemonImage);

            byte[]? cacheItem = null;
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetByNumberAsync(It.IsAny<int>());
            var obj = result as ObjectResult;

            obj.Should().BeNull();
        }

        [Fact]
        public async Task GetByNameAsync_Uncached_Returns_Ok()
        {
            var pokemonStats = _fixture.Create<PokemonStats>();
            _pokemonStatsRepo.Setup(repo => repo.GetStatsByNameAsync(It.IsAny<string>())).ReturnsAsync(pokemonStats);

            var pokemonImage = _fixture.Create<PokemonImage>();
            _pokemonImagesRepo.Setup(repo => repo.GetImageByNameAsync(It.IsAny<string>())).ReturnsAsync(pokemonImage);

            byte[]? cacheItem = null;
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetByNameAsync("testString");
            var obj = result as ObjectResult;

            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetByNameAsync_Cached_Returns_Ok()
        {
            var pokemon = _fixture.Create<Pokemon>();
            var cacheItem = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pokemon));
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetByNameAsync("testString");
            var obj = result as ObjectResult;

            obj.StatusCode.Should().Be(200);
        }

        // Test to ensure that if a valid name (ie. a value that exists in the Pokemon repos) is entered, the
        // response body is not empty.
        [Fact]
        public async Task GetByNameAsync_ExistingInputName_Returns_NonEmptyResult()
        {
            var pokemonStats = _fixture.Create<PokemonStats>();
            _pokemonStatsRepo.Setup(repo => repo.GetStatsByNameAsync(It.IsAny<string>())).ReturnsAsync(pokemonStats);

            var pokemonImage = _fixture.Create<PokemonImage>();
            _pokemonImagesRepo.Setup(repo => repo.GetImageByNameAsync(It.IsAny<string>())).ReturnsAsync(pokemonImage);

            byte[]? cacheItem = null;
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetByNameAsync("testString");
            var obj = result as ObjectResult;

            obj.Should().NotBeNull();
        }

        // Test to ensure that if a non-valid name (ie. a value that does not exist in the Pokemon repos) is entered, the
        // response body is empty.
        [Fact]
        public async Task GetByNameAsync_NonExistingInputName_Returns_EmptyResult()
        {
            PokemonStats? pokemonStats = null;
            _pokemonStatsRepo.Setup(repo => repo.GetStatsByNameAsync(It.IsAny<string>())).ReturnsAsync(pokemonStats);

            PokemonImage? pokemonImage = null;
            _pokemonImagesRepo.Setup(repo => repo.GetImageByNameAsync(It.IsAny<string>())).ReturnsAsync(pokemonImage);

            byte[]? cacheItem = null;
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetByNameAsync("testString");
            var obj = result as ObjectResult;

            obj.Should().BeNull();
        }

        [Fact]
        public async Task GetAllNamesNumsImagesAsync_Uncached_Returns_Ok_NonEmptyResult()
        {
            var allNamesNums = _fixture.Create<List<PokemonNamesNums>>();
            _pokemonStatsRepo.Setup(repo => repo.GetAllNamesAndPokedexNumsAsync()).ReturnsAsync(allNamesNums);

            var allImages = _fixture.Create<List<PokemonImage>>();
            _pokemonImagesRepo.Setup(repo => repo.GetAllImagesAsync()).ReturnsAsync(allImages);

            byte[]? cacheItem = null;
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetAllNamesNumsImagesAsync();
            var obj = result as ObjectResult;

            obj.StatusCode.Should().Be(200);
            obj.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllNamesNumsImagesAsync_Cached_Returns_Ok_NonEmptyResult()
        {
            var allNamesNumsImages = _fixture.Create<List<PokemonNamesNumsImages>>();
            var cacheItem = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(allNamesNumsImages));
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetAllNamesNumsImagesAsync();
            var obj = result as ObjectResult;

            obj.StatusCode.Should().Be(200);
            obj.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(ValidFilterQueries))]
        public async Task GetByFilterAsync_ValidKeysAndQueries_Uncached_Returns_Ok(string url)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(url);
            var queryParamsDictionary = nameValueCollection.AllKeys.ToDictionary(key => key, key => nameValueCollection.GetValues(key).ToList());

            byte[]? cacheItem = null;
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var stats = _fixture.Create<List<PokemonStats>>();
            _pokemonStatsRepo.Setup(repo => repo.GetStatsByAttributeAsync(It.IsNotNull<string>())).ReturnsAsync(stats);

            var result = await _controller.GetByFilterAsync(queryParamsDictionary);
            var obj = result as ObjectResult;

            obj.StatusCode.Should().Be(200);
        }

        [Theory]
        [MemberData(nameof(ValidFilterQueries))]
        public async Task GetByFilterAsync_ValidKeysAndQueries_Cached_Returns_Ok(string url)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(url);
            var queryParamsDictionary = nameValueCollection.AllKeys.ToDictionary(key => key, key => nameValueCollection.GetValues(key).ToList());

            var stats = _fixture.Create<List<PokemonStats>>();
            var cacheItem = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(stats));
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var result = await _controller.GetByFilterAsync(queryParamsDictionary);
            var obj = result as ObjectResult;

            obj.StatusCode.Should().Be(200);
        }

        [Theory]
        [MemberData(nameof(InvalidFilterQueries))]
        public async Task GetByFilterAsync_InvalidKeysAndQueries_Throws_Exception(string url)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(url);
            var queryParamsDictionary = nameValueCollection.AllKeys.ToDictionary(key => key, key => nameValueCollection.GetValues(key).ToList());

            byte[]? cacheItem = null;
            _pokemonCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(cacheItem);

            var stats = _fixture.Create<List<PokemonStats>>();
            _pokemonStatsRepo.Setup(repo => repo.GetStatsByAttributeAsync(It.IsNotNull<string>())).ReturnsAsync(stats);

            await Assert.ThrowsAnyAsync<Exception>(() => _controller.GetByFilterAsync(queryParamsDictionary));
        }
    }
}