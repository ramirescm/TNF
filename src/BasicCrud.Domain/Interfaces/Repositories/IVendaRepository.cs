using BasicCrud.Domain.Entities;
using System;
using System.Threading.Tasks;
using Tnf.Repositories;

namespace BasicCrud.Domain.Interfaces.Repositories
{
    // Para que essa interface seja registrada por convenção ela precisa herdar de alguma dessas interfaces: ITransientDependency, IScopedDependency, ISingletonDependency
    public interface IVendaRepository : IRepository
    {
        Task<Venda> InsertVendaAndGetIdAsync(Venda venda);

        Task<Venda> UpdateVendaAsync(Venda venda);

        Task DeleteVendaAsync(Guid id);
    }
}
