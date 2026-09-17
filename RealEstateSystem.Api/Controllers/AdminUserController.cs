using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var result = await _adminUserService.GetAllUsersAsync(cancellationToken);
            return Ok(result);
        }

        [HttpPost("{userId:guid}/lock")]
        public async Task<IActionResult> LockUser(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _adminUserService.LockUserAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{userId:guid}/unlock")]
        public async Task<IActionResult> UnlockUser(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _adminUserService.UnlockUserAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _adminUserService.DeleteUserAsync(userId, cancellationToken);
            if (!result)
            {
                return BadRequest(new { Message = "Xóa người dùng thất bại." });
            }

            return Ok(new { Message = "Xóa người dùng thành công." });
        }
    }
}
