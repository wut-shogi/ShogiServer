using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ShogiServer.WebApi.Hubs;
using ShogiServer.WebApi.Model;
using ShogiServer.WebApi.Services;
using System.Data.Common;
using System.Text.Json.Serialization;

namespace ShogiServer.WebApi
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection($"DataSource={Guid.NewGuid()};Mode=Memory;Cache=shared");
                connection.Open();
                return connection;
            });

            builder.Services.AddDbContext<DatabaseContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            builder.Services.AddSignalR(options => 
            {
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(15);
                options.KeepAliveInterval = TimeSpan.FromSeconds(60);
            });

            builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSignalRSwaggerGen();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<MatchmakingHub>("/matchmaking");
            app.MapHub<GameHub>("/game");

            app.Run();
        }
    }
}