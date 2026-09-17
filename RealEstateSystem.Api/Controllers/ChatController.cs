using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealEstateSystem.Api.Hubs;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateChatRoomRequest request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _chatService.CreateChatRoomAsync(userId, request.ReceiverId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _chatService.GetChatRoomsForUserAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("rooms/{roomId:guid}/messages")]
        public async Task<IActionResult> GetMessages(Guid roomId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _chatService.GetMessagesAsync(roomId, userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("rooms/{roomId:guid}/messages")]
        public async Task<IActionResult> SendMessage(Guid roomId, [FromBody] SendMessageRequest request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _chatService.SendMessageAsync(userId, roomId, request.Message, cancellationToken);

            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", result, cancellationToken);

            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Yêu cầu xác thực không hợp lệ.");
            }

            return userId;
        }
    }

    public record CreateChatRoomRequest(Guid ReceiverId);
    public record SendMessageRequest(string Message);
}
