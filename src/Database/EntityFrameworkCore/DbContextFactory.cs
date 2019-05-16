using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Database.EntityFrameworkCore
{
    public class DbContextFactory : IDesignTimeDbContextFactory<APIDbContext>
    {
        public APIDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<APIDbContext> builder = new DbContextOptionsBuilder<APIDbContext>();

            IConfigurationRoot configuration = Configuration.AppConfigurations.Get(Configuration.WebContentDirectoryFinder.CalculateContentRootFolder());

            DbContextConfigurer.Configure(builder, configuration.GetConnectionString("DefaultConnection"));

            return new APIDbContext(builder.Options);
        }
    }
}