using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PokemonApi.Middleware;
using System.Net;
using System.Text.Json;

namespace PokemonApi_Tests
{
    public class PokemonApi_MiddlewareTests
    {
        private readonly IHost _host;

        public PokemonApi_MiddlewareTests()
        {
            _host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddRouting();
                        })
                        .Configure(app =>
                        {
                            app.AddGlobalExceptionHandling();
                            app.UseRouting();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapGet("/valid-query-endpoint", context => context.Response.WriteAsync(JsonSerializer.Serialize("200 Ok response.")));
                                endpoints.MapGet("/invalid-query-endpoint", context => throw new Exception("Attempt to activate middleware exception handling."));
                            });
                        });
                })
                .Build();
        }

        [Fact]
        public async Task Get_ValidQueryEndpoint_Returns_Success_Middleware_NotActivated()
        {
            await _host.StartAsync();

            var response = await _host.GetTestClient().GetAsync("/valid-query-endpoint");

            response.IsSuccessStatusCode.Should().BeTrue();

            await _host.StopAsync();
        }

        [Fact]
        public async Task Get_InvalidQueryEndpoint_Returns_Error_Middleware_Activated()
        {
            await _host.StartAsync();

            var response = await _host.GetTestClient().GetAsync("/invalid-query-endpoint");

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var responseMsg = await response.Content.ReadAsStringAsync();
            responseMsg.Should().Be("{\"Error\":\"The application encountered an error while processing the request.\"}"); //{"Error":"The application encountered an error while processing the request."} -- Json response body

            await _host.StopAsync();
        }
    }
}