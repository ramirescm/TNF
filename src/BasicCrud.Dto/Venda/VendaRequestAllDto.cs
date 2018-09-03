using BasicCrud.Dto.Product;
using System;
using Tnf.Dto;

namespace BasicCrud.Dto.Venda
{
    public class VendaRequestAllDto : RequestAllDto
    {
        public int Quantidade { get; set; }
    }
}
