using BasicCrud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasicCrud.Infra.Context.Builders
{
    public class VendaTypeConfiguration : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            builder.ToTable("Vendas");

            builder.HasKey(k => k.Id);
            builder.HasOne<Product>(x => x.Product);
            builder.Property(p => p.Quantidade).IsRequired();
        }
    }
}
