using BasicCrud.Application.Services.Interfaces;
using BasicCrud.Domain.Entities;
using BasicCrud.Dto;
using BasicCrud.Dto.Venda;
using BasicCrud.Infra.ReadInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tnf.Application.Services;
using Tnf.Domain.Services;
using Tnf.Dto;
using Tnf.Notifications;

namespace BasicCrud.Application.Services
{
    public class VendaAppService : ApplicationService, IVendaAppService
    {
        private readonly IDomainService<Venda> _service;
        private readonly IVendaReadRepository _readRepository;

        public VendaAppService(IDomainService<Venda> service, IVendaReadRepository readRepository, INotificationHandler notificationHandler)
            : base(notificationHandler)
        {
            _service = service;
            _readRepository = readRepository;
        }

        public async Task<VendaDto> CreateVendaAsync(VendaDto dto)
        {
            if (!ValidateDto<VendaDto>(dto))
                return null;

            var builder = Venda.Create(Notification)
                .WithId(dto.Id)
                .WithProductId(dto.ProductId)
                .WithQuantidade(dto.Quantidade);

            var venda = await _service.InsertAndSaveChangesAsync(builder);

            return venda.MapTo<VendaDto>();
        }

        public async Task DeleteVendaAsync(Guid id)
        {
            if (!ValidateId(id))
                return;

            await _service.DeleteAsync(w => w.Id == id);
        }

        public async Task<VendaDto> GetVendaAsync(DefaultRequestDto id)
        {
            if (!ValidateRequestDto(id) || !ValidateId<Guid>(id.Id))
                return null;

            var entity = await _readRepository.GetVendaAsync(id);

            return entity.MapTo<VendaDto>();
        }

        public async Task<IListDto<VendaDto>> GetAllVendaAsync(VendaRequestAllDto request)
            => await _readRepository.GetAllVendaAsync(request);

        public async Task<VendaDto> UpdateVendaAsync(Guid id, VendaDto dto)
        {
            if (!ValidateDtoAndId(dto, id))
                return null;

            var builder = Venda.Create(Notification)
                .WithId(id)
                .WithProductId(dto.ProductId)
                .WithQuantidade(dto.Quantidade);

            await _service.UpdateAsync(builder);

            dto.Id = id;
            return dto;
        }
    }
}
