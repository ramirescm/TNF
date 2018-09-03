using BasicCrud.Application.Services.Interfaces;
using BasicCrud.Dto;
using BasicCrud.Dto.Venda;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tnf.AspNetCore.Mvc.Response;
using Tnf.Dto;

namespace BasicCrud.Web.Controllers
{
    /// <summary>
    /// Venda API
    /// </summary>
    [Route(WebConstants.VendaRouteName)]
    public class VendaController : TnfController
    {
        private readonly IVendaAppService _appService;
        private const string _name = "Venda";

        public VendaController(IVendaAppService appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// Get all Vendas
        /// </summary>
        /// <param name="requestDto">Request params</param>
        /// <returns>List of Vendas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IListDto<VendaDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAll([FromQuery]VendaRequestAllDto requestDto)
        {
            var response = await _appService.GetAllVendaAsync(requestDto);

            return CreateResponseOnGetAll(response, _name);
        }

        /// <summary>
        /// Get Venda
        /// </summary>
        /// <param name="id">Venda id</param>
        /// <param name="requestDto">Request params</param>
        /// <returns>Venda requested</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VendaDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Get(Guid id, [FromQuery]RequestDto requestDto)
        {
            var request = new DefaultRequestDto(id, requestDto);

            var response = await _appService.GetVendaAsync(request);

            return CreateResponseOnGet(response, _name);
        }

        /// <summary>
        /// Create a new Venda
        /// </summary>
        /// <param name="VendaDto">Venda to create</param>
        /// <returns>Venda created</returns>
        [HttpPost]
        [ProducesResponseType(typeof(VendaDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Post([FromBody]VendaDto VendaDto)
        {
            VendaDto = await _appService.CreateVendaAsync(VendaDto);

            return CreateResponseOnPost(VendaDto, _name);
        }

        /// <summary>
        /// Update a Venda
        /// </summary>
        /// <param name="id">Venda id</param>
        /// <param name="VendaDto">Venda content to update</param>
        /// <returns>Updated Venda</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(VendaDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Put(Guid id, [FromBody]VendaDto VendaDto)
        {
            VendaDto = await _appService.UpdateVendaAsync(id, VendaDto);

            return CreateResponseOnPut(VendaDto, _name);
        }

        /// <summary>
        /// Delete a Venda
        /// </summary>
        /// <param name="id">Venda id</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _appService.DeleteVendaAsync(id);

            return CreateResponseOnDelete(_name);
        }
    }
}