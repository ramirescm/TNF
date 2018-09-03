## Basic CRUD Sample

Exemplo que contempla um cen�rio de CRUD b�sico acessando um banco SqlServer, SqLite e Oracle.

Segue abaixo uma expli��o resumida de cada camada da aplica��o usando como exemplo o CRUD de Customer. 


### Camada de DTO

Essa camada guarda os objetos que a api ir� receber e mandar para respeitar o Guia de implementa��o das APIs TOTVS (http://tdn.totvs.com/pages/releaseview.action?pageId=271660444).

#### Configura��o de DTO

Esse camada precisa fazer refer�ncia as dll's <b>Tnf.Dto</b>, para utilizar os objetos de DTO disponibilizados pelo TNF.

#### Objetos de DTO

Objetos que a api ir� receber ou enviar precisam implementar a interface Tnf.Dto.IDto ou herdar de sua classe abstrata Tnf.Dto.DtoBase, como mostra o exemplo abaixo:

``` c#
public class CustomerDto : DtoBase<Guid>
{
  public static CustomerDto NullInstance = new CustomerDto().AsNullable<CustomerDto, Guid>();
  
  public string Name { get; set; }
}
```

Objetos de configura��es que iram retornar uma lista (como por exemplo um GetAll) e possuir�o um cmapo de busca precisam implementar a interface Tnf.Dto.IRequestAllDto ou herdar de sua classe abstrata Tnf.Dto.RequestAllDto, como mostra o exemplo abaixo:

``` c#
public class CustomerRequestAllDto : RequestAllDto
{
  public string Name { get; set; }
}
```


### Camada de Web

Essa camada possui a responsabilidade de levantar a sua aplica��o, configurar suas prefer�ncias (ex.: Log), registrar suas depend�ncias e exp�r suas api's.

#### Configura��o de Web

Esse camada precisa fazer refer�ncia as dll's <b>Tnf.Repositories.AspNetCore</b>, para utilizar o gerenciamento de api, classes de controllers e o pattern de UnitOfWork impl�cito.

Para que este exemplo funcione voc� precisa configurar uma instancia v�lida do SqlServer, SqLite ou Oracle nos config da aplica��o nos arquivos appsettings.Development.json e appsettings.Production.json.
	
Obs: Este exemplo utiliza as migra��es do EntityFrameWorkCore e para que elas possam ser executadas o usu�rio de seu SqlServer, SqLite ou Oracle ir� precisar de permiss�o para alterar a base de dados.

Para registrar as depend�ncias da aplica��o na classe Startup precisa chamar o registro de depend�ncias de cada camada, como mostra o exemplo abaixo:

``` c#
public class Startup
{
  public IServiceProvider ConfigureServices(IServiceCollection services)
  {
    services
      .AddApplicationServiceDependency()  // dependencia da camada BasicCrud.Application
      .AddSqlServerDependency()           // dependencia da camada BasicCrud.Infra.SqlServer
      .AddTnfAspNetCore();                // dependencia do pacote Tnf.AspNetCore

    return services.BuildServiceProvider();
  }
}
```

Para usar o pattern de UnitOfWork impl�cito e registrar suas configura��es de localiza��o tamb�m na classe Startup precisa configurar o m�todo Configure, como mostra o exemplo abaixo:

``` c#
public class Startup
{
  public void Configure(IApplicationBuilder app, IHostingEnvironment env)
  {
    // Configura o use do AspNetCore do Tnf
    app.UseTnfAspNetCore(options =>
    {
      // Adiciona as configura��es de localiza��o da aplica��o
      options.UseDomainLocalization();
      
      // Recupera a configura��o da aplica��o
      var configuration = options.Settings.FromJsonFiles(env.ContentRootPath, "appsettings.json");
      
      // Configura a connection string da aplica��o
      options.DefaultNameOrConnectionString = configuration.GetConnectionString(SqlServerConstants.ConnectionStringName);
    });
    
    // Habilita o uso do UnitOfWork em todo o request
    app.UseTnfUnitOfWork();
    
    app.UseMvc(routes =>
    {
      routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
    });
    
    app.Run(context =>
    {
      context.Response.Redirect("/swagger/ui");
      return Task.CompletedTask;
    });
  }
}
```

#### Configura��o das Controllers

Para exp�r uma api � necessario apenas que ela herde da classe Microsoft.AspNetCore.Mvc.TnfController, para que essa api respeite o padr�o do Guia de implementa��o das APIs TOTVS ela precisa receber nos m�todos as nossas interfaces de IRequestDto, IRequestAllDto e IDto, para que ele retorne tamb�m nesse padr�o uma mensagem de erro ou sucesso � necess�ri oque cada m�todo retorne o CreateResponse de cada verbo, como mostra o exemplo abaixo:

```c#
[Route(WebConstants.CustomerRouteName)]
public class CustomerController : TnfController
{
  private readonly ICustomerAppService appService;
  private const string name = "Customer";
  
  public CustomerController(ICustomerAppService appService)
  {
    this.appService = appService;
  }
  
  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery]CustomerRequestAllDto requestDto)
  {
    var response = await appService.GetAll(requestDto);
  
    return CreateResponseOnGetAll(response, name);
  }
  
  [HttpGet("{id}")]
  public async Task<IActionResult> Get(Guid id, [FromQuery]RequestDto<Guid> requestDto)
  {
    requestDto.WithId(id);
  
    var response = await appService.Get(requestDto);
  
    return CreateResponseOnGet<CustomerDto, Guid>(response, name);
  }
  
  [HttpPost]
  public async Task<IActionResult> Post([FromBody]CustomerDto customerDto)
  {
    customerDto = await appService.Create(customerDto);
  
    return CreateResponseOnPost<CustomerDto, Guid>(customerDto, name);
  }
  
  [HttpPut("{id}")]
  public async Task<IActionResult> Put(Guid id, [FromBody]CustomerDto customerDto)
  {
    customerDto = await appService.Update(id, customerDto);
  
    return CreateResponseOnPut<CustomerDto, Guid>(customerDto, name);
  }
  
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    await appService.Delete(id);
  
    return CreateResponseOnDelete<CustomerDto, Guid>(name);
  }
}
```


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


### Camada de Infra

Essa camada possui a responsabilidade de fazer o gerenciamento do banco, ou seja, fazer a integra��o com o banco de dados de sua prefer�ncia, mapear as entidades que ir�o ser persisitidas e buscadas no banco e armazenar a l�gica de persist�ncia e busca.

Obs: Nesse resumo mostraramos como faz a configura��o dessa camada usando nossa abstra��o para SqlServer.

#### Configura��o de Infra

Esse camada precisa fazer refer�ncia as dll's <b>Tnf.AutoMapper</b>, para utilizar o mapeamento autom�tico que o TNF disponibiliza, e <b>Tnf.EntityFrameworkCore</b>, para utilizar o EntityFramework para aplicar a l�gica de persist�ncia e busca ao banco de dados.

Para fazer com que essa camada utilize as configura��es do EntityFrameworkCore do TNF � necess�rio chamar o m�todo AddTnfEntityFrameworkCore da interface Microsoft.Extensions.DependencyInjection.IServiceCollection, como mostra o exemplo abaixo:

```c#
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddInfraDependency(this IServiceCollection services)
  {
    return services
      .AddTnfEntityFrameworkCore()    // Configura o uso do EntityFrameworkCore registrando os contextos que ser�o usados pela aplica��o
      .AddMapperDependency();         // Configura o uso do AutoMappper
  }
}
```

Como todas camadas � preciso registrar as suas depend�ncias, contratos e implementa��es, para isso � ncess�rio criar um m�todo que extenda (ou receba) a interface Microsoft.Extensions.DependencyInjection.IServiceCollection para que as camadas que possuem depend�ncia para essa camada possam registr�-la.

Nessa camada tamb�m � necess�rio informar o contexto que a aplica��o ir� usar e com qual string de conex�o, como mostra o exemplo abaixo:

```c#
public static IServiceCollection AddSqlServerDependency(this IServiceCollection services)
{
  services
    .AddInfraDependency()
    .AddTnfDbContext<BasicCrudDbContext>((config) =>
    {
      if (config.ExistingConnection != null)
        config.DbContextOptions.UseSqlServer(config.ExistingConnection);
      else
        config.DbContextOptions.UseSqlServer(config.ConnectionString);
    });


  // Registro dos reposit�rios
  services.AddTransient<IProductRepository, ProductRepository>();
  services.AddTransient<IProductReadRepository, ProductReadRepository>();

  return services;
}
```

Obs: Nesse exemplo � mostrado como se configura o contexto com o banco de dados SqlServer por isso ao adicionar a string de conex�o usamos o m�todo UseSqlServer que est� na dll <b>Microsoft.EntityFrameworkCore.SqlServer</b>, se estiver usando Oracle o m�todo ser� UseOracle da dll <b>Devart.Data.Oracle.Entity.EFCore</b> e se estiver usando SqLite o m�todo ser� UseSqlite da dll <b>Microsoft.EntityFrameworkCore.Sqlite</b>.
 
#### Configura��o de Mapeamento

Para configurar o mapeamento na sua aplica��o � necess�rio criar um m�todo que extenda (ou receba) a interface Tnf.Configuration.ITnfConfiguration para que as camadas que forem depender dessa possam registrar essa configura��o, como mostra o exemplo abaixo:

```c#
public static class MapperExtensions
{
  public static IServiceCollection AddMapperDependency(this IServiceCollection services)
  {
    // Configura o uso do AutoMappper
    return services.AddTnfAutoMapper(config =>
    {
      config.AddProfile<BasicCrudProfile>();
    });
  }
}
```

Abaixo segue o exemplo do profile que mostra o mapeamento das entidades:

```c#
public class BasicCrudProfile : Profile
{
  public BasicCrudProfile()
  {
    CreateMap<Customer, CustomerDto>();
  }
}
```

Obs: Na configura��o de mapeamentos usamos o framework AutoMapper para isso, se precisar fazer um mapeamento mais complexo segue a documenta��o: http://docs.automapper.org/en/stable/Configuration.html.

#### Configura��o de Contexto

O contexto vai definir as tabelas do banco da aplica��o e o mapeamento do objeto da aplica��o para o registro do banco, como mostra o exemplo abaixo:

```c#
public class BasicCrudDbContext : TnfDbContext
{
  public DbSet<Customer> Customers { get; set; }

  // Importante o construtor do contexto receber as op��es com o tipo generico definido: DbContextOptions<TDbContext>
  public BasicCrudDbContext(DbContextOptions<BasicCrudDbContext> options, ITnfSession session) 
    : base(options, session)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfiguration(new CustomerTypeConfiguration());
  }
}
```

Para configurar o mapeamento do objeto da aplica��o para a tabela do banco de dados recomendamos que usem a estrat�gia do pr�prio EntityFramework de separar em uma classe que implementa a interface Microsoft.EntityFrameworkCore.IEntityTypeConfiguration, como mostra o exemplo abaixo:

```c#
public class CustomerTypeConfiguration : IEntityTypeConfiguration<Customer>
{
  public void Configure(EntityTypeBuilder<Customer> builder)
  {
    builder.HasKey(k => k.Id);
    builder.Property(p => p.Name).IsRequired();
  }
}
```

#### Utiliza��o de Migrations

Nesse exemplo usamos a t�tica de modelagem de Code First, onde escrevemos o c�digo para gerar as tabelas do banco em cima dele, com EntityFramework fazemos uso de migration's que atualizam o banco para n�s, para isso precisamos criar um Factory que ser� chamado ao executar a migration e retornar� uma inst�ncia do contexto, como mostra o exemplo abaixo:

```c#
public class BasicCrudDbContextFactory : IDesignTimeDbContextFactory<BasicCrudDbContext>
{
  public BasicCrudDbContext CreateDbContext(string[] args)
  {
    var builder = new DbContextOptionsBuilder<BasicCrudDbContext>();

    var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true)
                .Build();
    
    builder.UseSqlServer(configuration.GetConnectionString(SqlServerConstants.ConnectionStringName));

    return new BasicCrudDbContext(builder.Options, NullTnfSession.Instance);
  }
}
```

Para executar migrations agora � necess�rio abrir a janela do Package Manager Console dentro de seu Visual Studio, colocar o seu projeto de endpoint como StartUp Project, no nosso caso seria o projeto BasicCrud.Web, selecionar o Default Project para o projeto que possui o Factory e o DbContext, no nosso caso seria o projeto BasicCrud.Infra.SqlServer, e executar o seguinte comando:

	Add-Migration "NomeDaMigration"

Fazendo isso a migration ser� criada e falta apenas atualizar o banco de dados com o seguinte comando:

	Update-Database
