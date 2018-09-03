using BasicCrud.Dto.Product;
using System;
using Tnf.Dto;

namespace BasicCrud.Dto.Venda
{
    public class VendaDto : BaseDto
    {
        public Guid Id { get; set; }
        public ProductDto Product { get; set; }
        public Guid ProductId { get; set; }
        public int Quantidade { get; set; }
    }
}
