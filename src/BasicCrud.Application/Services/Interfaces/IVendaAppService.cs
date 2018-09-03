using BasicCrud.Dto;
using BasicCrud.Dto.Venda;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tnf.Application.Services;
using Tnf.Dto;

namespace BasicCrud.Application.Services.Interfaces
{
    public interface IVendaAppService : IApplicationService
    {
        Task<VendaDto> CreateVendaAsync(VendaDto customerDto);
        Task<VendaDto> UpdateVendaAsync(Guid id, VendaDto customerDto);
        Task DeleteVendaAsync(Guid id);
        Task<IListDto<VendaDto>> GetAllVendaAsync(VendaRequestAllDto request);
        Task<VendaDto> GetVendaAsync(DefaultRequestDto request);
    }
}
