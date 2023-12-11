using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ShogiServer.WebApi;
using ShogiServer.WebApi.Services;

namespace ShogiServer.IntegrationTests
{

    internal class StartupTests
    {
        private WebApplicationFactory<Program> application = null!;
        private IServiceScope scope = null!;

        [SetUp]
        public void Setup()
        {
            application = new WebApplicationFactory<Program>();
            scope = application.Services.CreateScope();
        }

        [Test]
        public void Startup_Succeeds()
        {
            application.Should().NotBeNull();
        }

        [Test]
        public void IRepositoryWrapper_Exists()
        {
            scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>().Should().NotBeNull();
        }
    }
}