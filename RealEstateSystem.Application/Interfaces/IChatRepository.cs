using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatRoom?> GetChatRoomByIdAsync(Guid chatRoomId, CancellationToken cancellationToken);
        Task<ChatRoom?> GetChatRoomByParticipantsAsync(Guid senderId, Guid receiverId, CancellationToken cancellationToken);
        Task<List<ChatRoom>> GetChatRoomsForUserAsync(Guid userId, CancellationToken cancellationToken);
        Task AddChatRoomAsync(ChatRoom chatRoom, CancellationToken cancellationToken);
        Task AddChatMessageAsync(ChatMessage message, CancellationToken cancellationToken);
        Task<List<ChatMessage>> GetMessagesForRoomAsync(Guid chatRoomId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
