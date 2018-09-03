using BasicCrud.Domain.Entities;
using BasicCrud.Dto;
using BasicCrud.Dto.Venda;
using BasicCrud.Infra.Context;
using BasicCrud.Infra.ReadInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tnf.Dto;
using Tnf.EntityFrameworkCore;
using Tnf.EntityFrameworkCore.Repositories;

namespace BasicCrud.Infra.Repositories.ReadRepositories
{
    public class VendaReadRepository : EfCoreRepositoryBase<CrudDbContext, Product>, IVendaReadRepository
    {
        public VendaReadRepository(IDbContextProvider<CrudDbContext> dbContextProvider) 
            : base(dbContextProvider)
        {
        }

        public async Task<IListDto<VendaDto>> GetAllVendaAsync(VendaRequestAllDto key)
        {
            return Context.Vendas.Include("Product").MapTo<List<VendaDto>>().ToListDto(false);
        }

        public async Task<VendaDto> GetVendaAsync(DefaultRequestDto key)
        {
            return Context.Vendas.Include("Product").Where(x => x.Id == key.Id).FirstOrDefault().MapTo<VendaDto>();
        }
    }
}
