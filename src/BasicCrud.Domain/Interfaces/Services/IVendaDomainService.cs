﻿using BasicCrud.Domain.Entities;
using System;
using System.Threading.Tasks;
using Tnf.Domain.Services;

namespace BasicCrud.Domain.Interfaces.Services
{
    // Para que essa interface seja registrada por convenção ela precisa herdar de alguma dessas interfaces: ITransientDependency, IScopedDependency, ISingletonDependency
    public interface IVendaDomainService : IDomainService
    {
        Task<Venda> InsertVendaAsync(Venda.Builder builder);

        Task<Venda> UpdateVendaAsync(Venda.Builder builder);

        Task DeleteVendaAsync(Guid id);
    }
}
