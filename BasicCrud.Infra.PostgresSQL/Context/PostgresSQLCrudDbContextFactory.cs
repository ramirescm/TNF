using BasicCrud.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Tnf.Runtime.Session;

namespace BasicCrud.Infra.SqlServer.Context
{
    public class PostgresSQLCrudDbContextFactory : IDesignTimeDbContextFactory<PostgresSQLCrudDbContext>
    {
        public PostgresSQLCrudDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CrudDbContext>();

            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile($"appsettings.Development.json", false)
                                    .Build();

            var databaseConfiguration = new DatabaseConfiguration(configuration);

            builder.UseNpgsql(databaseConfiguration.ConnectionString);

            return new PostgresSQLCrudDbContext(builder.Options, NullTnfSession.Instance);
        }
    }
}
