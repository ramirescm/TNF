using BasicCrud.Domain.Entities;
using BasicCrud.Domain.Interfaces.Repositories;
using BasicCrud.Domain.Interfaces.Services;
using System;
using System.Threading.Tasks;
using Tnf.Domain.Services;
using Tnf.Notifications;

namespace BasicCrud.Domain.Services
{
    public class VendaDomainService : DomainService, IVendaDomainService
    {
        private readonly IVendaRepository _repository;

        public VendaDomainService(IVendaRepository repository, INotificationHandler notificationHandler)
            : base(notificationHandler)
        {
            _repository = repository;
        }

        public Task DeleteVendaAsync(Guid id) => _repository.DeleteVendaAsync(id);

        public async Task<Venda> InsertVendaAsync(Venda.Builder builder)
        {
            if (builder == null)
            {
                Notification.RaiseError(Constants.LocalizationSourceName, Error.DomainServiceOnInsertNullBuilderError);
                return default(Venda);
            }

            var Venda = builder.Build();

            if (Notification.HasNotification())
                return default(Venda);

            Venda = await _repository.InsertVendaAndGetIdAsync(Venda);

            return Venda;
        }

        public async Task<Venda> UpdateVendaAsync(Venda.Builder builder)
        {
            if (builder == null)
            {
                Notification.RaiseError(Constants.LocalizationSourceName, Error.DomainServiceOnUpdateNullBuilderError);
                return default(Venda);
            }

            var Venda = builder.Build();

            if (Notification.HasNotification())
                return default(Venda);

            return await _repository.UpdateVendaAsync(Venda);
        }
    }
}
