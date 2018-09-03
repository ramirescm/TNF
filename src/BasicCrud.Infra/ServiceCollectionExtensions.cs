using BasicCrud.Domain.Interfaces.Repositories;
using BasicCrud.Infra.ReadInterfaces;
using BasicCrud.Infra.Repositories.ReadRepositories;
using Microsoft.Extensions.DependencyInjection;

namespace BasicCrud.Infra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraDependency(this IServiceCollection services)
        {
            services
                .AddTnfEntityFrameworkCore()    // Configura o uso do EntityFrameworkCore registrando os contextos que serão usados pela aplicação
                .AddMapperDependency();         // Configura o uso do AutoMappper

            // Registro dos repositórios

            // Write and updates
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IVendaRepository, VendaRepository>();
            // Reads
            services.AddTransient<IProductReadRepository, ProductReadRepository>();
            services.AddTransient<IVendaReadRepository, VendaReadRepository>();

            return services;
        }
    }
}
