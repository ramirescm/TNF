### Camada de Domain

Essa camada possui a responsabilidade de aplicar as regras de neg�cio da aplica��o.

#### Configura��o de Domain

Esse camada precisa fazer refer�ncia as dll's <b>Tnf.Domain</b>, para utilizar os pattern's de Builder e Specification e configura��es de localiza��o.

Para registra as depend�ncias da camada de dom�nio sendo elas usando dom�nio padr�o do TNF ou customizado, � ncess�rio criar um m�todo que extenda (ou receba) a interface Microsoft.Extensions.DependencyInjection.IServiceCollection para que as camadas que possuem depend�ncia para essa camada possam registr�-la, como mostra o exemplo abaixo:

```c#
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddApplicationServiceDependency(this IServiceCollection services)
  {
    // Adiciona as dependencias para utiliza��o dos servi�os de crud generico do Tnf
    services.AddTnfDomain();
    
    // Para habilitar as conven��es do Tnf para Inje��o de depend�ncia (ITransientDependency, IScopedDependency, ISingletonDependency)
    // descomente a linha abaixo:
    // services.AddTnfDefaultConventionalRegistrations();
    
    // Registro dos servi�os
    services.AddTransient<IProductDomainService, ProductDomainService>();
    
    return services;
  }
}
```

#### Configura��o de Localiza��o

Para configurar a localiza��o na sua aplica��o � necess�rio criar um m�todo que extenda (ou receba) a interface Tnf.Configuration.ITnfConfiguration para que as camadas que forem depender dessa possam registrar essa configura��o, como mostra o exemplo abaixo:

```c#
public static class LocalizationExtensions
{
  public static void UseDomainLocalization(this ITnfConfiguration configuration)
  {
    // Incluindo o source de localiza��o
    configuration.Localization.Sources.Add(
      new DictionaryBasedLocalizationSource(DomainConstants.LocalizationSourceName,
      new JsonEmbeddedFileLocalizationDictionaryProvider(
        typeof(DomainConstants).Assembly, 
        "BasicCrud.Domain.Localization.SourceFiles")));
    
    // Incluindo suporte as seguintes linguagens
    configuration.Localization.Languages.Add(new LanguageInfo("pt-BR", "Portugu�s", isDefault: true));
    configuration.Localization.Languages.Add(new LanguageInfo("en", "English"));
  }
}
```

#### Cria��o de Entidades utilizando os pattern's Builder e Specification

A camada de dom�nio n�o pode depender de nenhuma outra camada pois nela est� a regra de neg�cio e se no futuro as outras camadas forem trocadas ou sofrerem altera��es as suas regras de neg�cio devem ficar intactas na camada de dom�nio. 

Essa camada precisa saber criar suas entidades de neg�cio e aplicar regras de valida��o para seu neg�cio, para resolver isso o TNF faz uso dos pattern's de Builder e Specification.

Entidades de neg�cio precisam implementar a interface Tnf.Repositories.Entities.IEntity ou herdar de sua classe abstrata Tnf.Repositories.Entities.Entity.

Recomendamos que a entidade de dom�nio possua o builder que ir� constru�-la e que s� possa construir essa entidade atrav�s desse Builder, para que n�o possa criar uma entidade sem ter sido aplicado suas regras de neg�cio, esse builder precisa implementar a interface Tnf.Builder.IBuilder ou herdar de sua classe abstrata Tnf.Builder.Builder, como mostra o exemplo abaixo:

``` c#
public class Customer : Entity<Guid>
{
  public string Name { get; internal set; }
  
  public enum Error
  {
    CustomerShouldHaveName
  }
  
  public static CustomerBuilder Create(INotificationHandler handler)
    => new CustomerBuilder(handler);
  
  public static CustomerBuilder Create(INotificationHandler handler, Customer instance)
    => new CustomerBuilder(handler, instance);
  
  public class CustomerBuilder : Builder<Customer>
  {
    public CustomerBuilder(INotificationHandler notificationHandler)
      : base(notificationHandler)
    {
    }
  
    public CustomerBuilder(INotificationHandler notificationHandler, Customer instance)
      : base(notificationHandler, instance)
    {
    }
  
    public CustomerBuilder WithId(Guid id)
    {
      Instance.Id = id;
      return this;
    }
  
    public CustomerBuilder WithName(string name)
    {
      Instance.Name = name;
      return this;
    }
  
    protected override void Specifications()
    {
      AddSpecification<CustomerShouldHaveNameSpecification>();
    }
  }
}
```

Podemos notar que adicionamos a especifica��o CustomerShouldHaveNameSpecification no builder de customer, essa especifica��o vai ser executada ao chamar o build da classe e se ela n�o for satisfeita ela levantar� uma notifica��o com a chave passada para ela, essa especifica��o precisa implementar a interface Tnf.Specifications.ISpecification ou herdar de alguma de suas abstratas como Tnf.Specifications.Specification, como mostra o exemplo abaixo:

```c#
public class CustomerShouldHaveNameSpecification : Specification<Customer>
{
  public override string LocalizationSource { get; protected set; } = DomainConstants.LocalizationSourceName;
  public override Enum LocalizationKey { get; protected set; } = Customer.Error.CustomerShouldHaveName;
  
  public override Expression<Func<Customer, bool>> ToExpression()
  {
    return (p) => !string.IsNullOrWhiteSpace(p.Name);
  }
}
```

#### Inje��o de dom�nio e reposit�rio padr�o do TNF (CRUD de Customer):
	
No CRUD de Customer criamos um exemplo de como utilizar a inje��o de dom�nio e reposit�rio padr�o do TNF onde o desenvolvedor n�o precisa configurar uma classe de dom�nio nem uma classe de reposit�rio para sua aplica��o, e suas regras de neg�cio seriam executadas atrav�s dos pattern's de Specification e Builder.

Para isso a sua camada de Application precisa injetar a interface Tnf.Domain.Services.IDomainService explicitando a entidade de neg�cio e sua chave prim�ria, como mostra o exemplo abaixo:

``` c#
public class CustomerAppService : ApplicationService, ICustomerAppService
{
  private readonly IDomainService<Customer, Guid> service;
  
  public CustomerAppService(IDomainService<Customer, Guid> service, INotificationHandler notificationHandler)
 	 : base(notificationHandler)
  {
    this.service = service;
  }
}
```

Fazendo isso o TNF ir� injetar uma classe de dom�nio pr�pria com os m�todos de CRUD s�ncrono e ass�ncrono respeitando o pattern de Builder.

#### Inje��o de dom�nio e reposit�rio customizada (CRUD de Product):
	
No CRUD de Product criamos um exemplo de como utilizar a inje��o de dom�nio e reposit�rio customizada onde o desenvolvedor configura uma implementa��o de dom�nio e de reposit�rio para sua aplica��o e pode executar suas regras de neg�cio na classe de dom�nio e as regras de propriedades utilizando os pattern's de Specification e Builder.