using BasicCrud.Domain.Entities;
using BasicCrud.Domain.Interfaces.Repositories;
using BasicCrud.Infra.Context;
using System;
using System.Threading.Tasks;
using Tnf.EntityFrameworkCore;
using Tnf.EntityFrameworkCore.Repositories;

namespace BasicCrud.Infra
{
    public class VendaRepository : EfCoreRepositoryBase<CrudDbContext, Venda>, IVendaRepository
    {
        public VendaRepository(IDbContextProvider<CrudDbContext> dbContextProvider) 
            : base(dbContextProvider)
        {
        }

        public async Task DeleteVendaAsync(Guid id)
            => await DeleteAsync(w=>w.Id == id);

        public async Task<Venda> InsertVendaAndGetIdAsync(Venda Venda)
            => await InsertAndSaveChangesAsync(Venda);

        public async Task<Venda> UpdateVendaAsync(Venda Venda)
            => await UpdateAsync(Venda);
    }
}
