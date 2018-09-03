using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicCrud.Application.Services.Interfaces;
using BasicCrud.Dto.Venda;
using Microsoft.AspNetCore.Mvc;
using Tnf.AspNetCore.Mvc.Response;
using Tnf.Dto;

namespace BasicCrud.Web.Controllers
{
    [Route(WebConstants.TesteRouteName)]
    public class TesteController : TnfController
    {
        private const string _name = "Teste";
        private readonly IVendaAppService _appService;

        public TesteController(IVendaAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IListDto<VendaDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAll([FromQuery]VendaRequestAllDto requestDto)
        {
            var lista = new ListDto<VendaDto>();

            try
            {
                var response = await _appService.GetAllVendaAsync(requestDto);
            }
            catch(Exception ex)
            {
                var v = new VendaDto()
                {
                    Id = new Guid("0D4090B2-5687-4938-ABDF-A8DA016EE645"),
                    Product = new Dto.Product.ProductDto() { Id = new Guid("0D4090B2-5687-4938-ABDF-A8DA016EE645"), Description = ex.ToString() },
                    ProductId = new Guid("0D4090B2-5687-4938-ABDF-A8DA016EE645"),
                    Quantidade = 5
                };

                lista.Items.Add(v);
            }
            return CreateResponseOnGetAll(lista, _name);
        }

        
    }
}