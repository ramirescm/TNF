using AutoMapper;
using BasicCrud.Domain.Entities;
using BasicCrud.Dto.Customer;
using BasicCrud.Dto.Product;
using BasicCrud.Dto.Venda;

namespace BasicCrud.Infra.MapperProfiles
{
    public class BasicCrudProfile : Profile
    {
        public BasicCrudProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<Product, ProductDto>();
            CreateMap<Venda, VendaDto>();
        }
    }
}
