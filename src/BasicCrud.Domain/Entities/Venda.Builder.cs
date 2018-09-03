using System;
using Tnf.Builder;
using Tnf.Notifications;

namespace BasicCrud.Domain.Entities
{
    public partial class Venda
    {
        public class Builder : Builder<Venda>
        {
            public Builder(INotificationHandler notificationHandler)
                : base(notificationHandler)
            {
            }

            public Builder(INotificationHandler notificationHandler, Venda instance)
                : base(notificationHandler, instance)
            {
            }

            public Builder WithId(Guid id)
            {
                Instance.Id = id;
                return this;
            }

            public Builder WithProduct(Product product)
            {
                Instance.Product = product;
                return this;
            }

            public Builder WithProductId(Guid productId)
            {
                Instance.ProductId = productId;
                return this;
            }

            public Builder WithQuantidade(int quantidade)
            {
                Instance.Quantidade = quantidade;
                return this;
            }
        }
    }
}
