using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GhandiBot.Data
{
    /// <summary>
    /// Provided for dotnet ef migrations to build AppDbContext
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(GetConnectionString());
            
            return new AppDbContext(optionsBuilder.Options);
        }

        private string GetConnectionString()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.connection.development.json", false, true)
                .Build();

            return configurationBuilder.GetConnectionString("DefaultConnection");
        }
    }
}