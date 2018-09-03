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