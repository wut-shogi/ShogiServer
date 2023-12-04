using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ShogiServer.WebApi;
using ShogiServer.WebApi.Repositories;

namespace ShogiServer.IntegrationTests
{

    internal class StartupTests
    {
        private WebApplicationFactory<Program> application = null!;

        [SetUp]
        public void Setup()
        {
            application = new WebApplicationFactory<Program>();
        }

        [Test]
        public void Startup_Succeeds()
        {
            application.Should().NotBeNull();
        }

        [Test]
        public void ILobbyRepository_Exists()
        {
            application.Services.GetService<ILobbyRepository>().Should().NotBeNull();
        }
    }
}