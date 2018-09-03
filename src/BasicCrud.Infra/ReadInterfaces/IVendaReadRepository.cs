using BasicCrud.Dto;
using BasicCrud.Dto.Venda;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tnf.Dto;
using Tnf.Repositories;

namespace BasicCrud.Infra.ReadInterfaces
{
    // Para que essa interface seja registrada por convenção ela precisa herdar de alguma dessas interfaces: ITransientDependency, IScopedDependency, ISingletonDependency
    public interface IVendaReadRepository : IRepository
    {
        Task<VendaDto> GetVendaAsync(DefaultRequestDto key);
        Task<IListDto<VendaDto>> GetAllVendaAsync(VendaRequestAllDto key);
    }
}
