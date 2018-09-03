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
