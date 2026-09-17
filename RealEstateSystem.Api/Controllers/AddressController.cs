using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync(CancellationToken cancellationToken)
        {
            await _addressService.SyncProvincesAndWardsAsync(cancellationToken);
            return Ok(new { message = "Đồng bộ danh sách tỉnh thành và phường xã thành công." });
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces(CancellationToken cancellationToken)
        {
            var result = await _addressService.GetProvincesAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("provinces/{provinceId:guid}/wards")]
        public async Task<IActionResult> GetWards(Guid provinceId, CancellationToken cancellationToken)
        {
            var result = await _addressService.GetWardsByProvinceIdAsync(provinceId, cancellationToken);
            return Ok(result);
        }
    }
}
