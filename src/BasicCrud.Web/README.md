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