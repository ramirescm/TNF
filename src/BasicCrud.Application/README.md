### Camada de Application

Essa camada possui a responsabilidade de validar a integridade dos dados que o endpoint da solu��o recebe.

#### Configura��o de Application

Como todas camadas � preciso registrar as suas depend�ncias, contratos e implementa��es, para isso � ncess�rio criar um m�todo que extenda (ou receba) a interface Microsoft.Extensions.DependencyInjection.IServiceCollection para que as camadas que possuem depend�ncia para essa camada possam registr�-la, como mostra o exemplo abaixo:

```c#
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddApplicationServiceDependency(this IServiceCollection services)
  {
    // Depend�ncia do projeto BasicCrud.Domain
    services.AddDomainDependency();
    
    // Para habilitar as conven��es do Tnf para Inje��o de depend�ncia (ITransientDependency, IScopedDependency, ISingletonDependency)
    // descomente a linha abaixo:
    // services.AddTnfDefaultConventionalRegistrations();
    
    // Registro dos servi�os
    services.AddTransient<ICustomerAppService, CustomerAppService>();
    
    return services;
  }
}
```

Obs: Este exemplo n�o utiliza registro de depend�ncia por conven��o, ele registra todas as suas interfaces e implementa��es, para habilitar a conven��o em cada m�todo de depend�ncia de cada camada � preciso adicionar a linha comentada acima e nas interfaces registradas por conve��o precisam herdar de Tnf.Dependency.ITransientDependency, Tnf.Dependency.IScopedDependency ou Tnf.Dependency.ISingletonDependency de cordo com o seu tempo de vida na aplica��o.