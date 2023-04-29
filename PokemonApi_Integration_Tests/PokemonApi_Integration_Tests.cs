using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PokemonApi;

namespace PokemonApi_Integration_Tests
{
    public class PokemonApi_Integration_Tests : IClassFixture<WebApplicationFactory<Program>>
    {
        // Note: Swagger and Watchdog must be disabled in Program.cs file during integration tests.
        // Note: All database/repo instances must be running during integration tests.

        private readonly WebApplicationFactory<Program> _factory;

        public PokemonApi_Integration_Tests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ApiEndpoint_namesNumsImages_Should_Return_Ok_NotEmptyResponse()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/pokemon/namesNumsImages");
            var responseMsg = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            responseMsg.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task ApiEndpoint_pokedexnumber_Should_Return_Ok_NotEmptyResponse(int pokedexnum)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/pokemon/pokedexnumber/{pokedexnum}");
            var responseMsg = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            responseMsg.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1000000000)]
        [InlineData(-10)]
        public async Task ApiEndpoint_pokedexnumber_Should_Return_Ok_EmptyResponse(int pokedexnum)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/pokemon/pokedexnumber/{pokedexnum}");
            var responseMsg = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            responseMsg.Should().BeEmpty();
        }

        [Theory]
        [InlineData("bulbasaur")]
        [InlineData("Pikachu")]
        [InlineData("mewTWO")]
        [InlineData("RaYqUaZa")]
        public async Task ApiEndpoint_name_Should_Return_Ok_NotEmptyResponse(string name)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/pokemon/name/{name}");
            var responseMsg = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            responseMsg.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("definitely-not-in-database")]
        [InlineData("123456")]
        public async Task ApiEndpoint_name_Should_Return_Ok_EmptyResponse(string name)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/pokemon/name/{name}");
            var responseMsg = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            responseMsg.Should().BeEmpty();
        }

        [Theory]
        [InlineData("pokedex_number=LESS_THAN&pokedex_number=10")]
        [InlineData("height_m=GREATER_THAN&height_m=0.5")]
        [InlineData("name=CONTAINS&name=ka")]
        [InlineData("is_legendary=TRUE&is_legendary=")]
        public async Task ApiEndpoint_filter_Should_Return_Ok_NotEmptyResponse(string queryParams)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/pokemon/filter?{queryParams}");
            var responseMsg = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            responseMsg.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("pokedex_number=LESS_THAN&pokedex_number=0")]
        [InlineData("height_m=GREATER_THAN&height_m=1000000000")]
        [InlineData("name=EQUALS&name=NOT_A_NAME")]
        [InlineData("name=CONTAINS&name=abcd&pokedex_number=GREATER_THAN&pokedex_number=10&against_dark=LESS_THAN_OR_EQUALS&against_dark=2.5&is_legendary=TRUE&is_legendary=")]
        public async Task ApiEndpoint_filter_Should_Return_Ok_EmptyResponse(string queryParams)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/pokemon/filter?{queryParams}");
            var responseMsg = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            responseMsg.Should().Be("[]"); // Empty Ok response.
        }

        [Theory]
        [InlineData("non_existent_key=EQUALS&non_existent_key=1")]
        [InlineData("pokedex_number=GREATER_THAN&name=1")]
        [InlineData("pokedex_number=EQUALS")]
        [InlineData("is_legendary=NOT_A_FILTER_CONDITION&is_legendary=")]
        public async Task ApiEndpoint_filter_Should_Return_InternalServerError_CustomErrorResponse(string queryParams)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/pokemon/filter?{queryParams}");
            var responseMsg = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
            responseMsg.Should().Be("{\"Error\":\"The application encountered an error while processing the request.\"}"); //{"Error":"The application encountered an error while processing the request."} -- Json response body
        }
    }
}