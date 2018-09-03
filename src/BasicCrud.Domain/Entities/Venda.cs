using System;
using Tnf.Notifications;

namespace BasicCrud.Domain.Entities
{
    public partial class Venda : IEntity
    {
        public Guid Id { get; set; }

        public Product Product { get; internal set; }
        public Guid ProductId { get; internal set; }

        public int Quantidade { get; internal set; }

        public static Builder Create(INotificationHandler handler)
            => new Builder(handler);

        public static Builder Create(INotificationHandler handler, Venda instance)
            => new Builder(handler, instance);
    }
}
