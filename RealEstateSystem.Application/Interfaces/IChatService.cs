using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatRoomResponse> CreateChatRoomAsync(Guid senderId, Guid receiverId, CancellationToken cancellationToken);
        Task<List<ChatRoomResponse>> GetChatRoomsForUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<ChatMessageResponse>> GetMessagesAsync(Guid chatRoomId, Guid userId, CancellationToken cancellationToken);
        Task<ChatMessageResponse> SendMessageAsync(Guid senderId, Guid chatRoomId, string messageContent, CancellationToken cancellationToken);
    }
}
