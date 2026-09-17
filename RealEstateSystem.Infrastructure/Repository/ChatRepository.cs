using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Infrastructure.Persistence;

namespace RealEstateSystem.Infrastructure.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatRoom?> GetChatRoomByIdAsync(Guid chatRoomId, CancellationToken cancellationToken)
        {
            return await _context.ChatRooms
                .Include(r => r.Sender)
                .Include(r => r.Receiver)
                .Include(r => r.ChatMessages)
                .FirstOrDefaultAsync(r => r.ChatRoomId == chatRoomId && !r.IsDeleted, cancellationToken);
        }

        public async Task<ChatRoom?> GetChatRoomByParticipantsAsync(Guid senderId, Guid receiverId, CancellationToken cancellationToken)
        {
            return await _context.ChatRooms
                .Include(r => r.Sender)
                .Include(r => r.Receiver)
                .Include(r => r.ChatMessages)
                .FirstOrDefaultAsync(r => 
                    ((r.SenderId == senderId && r.ReceiverId == receiverId) || 
                     (r.SenderId == receiverId && r.ReceiverId == senderId)) && 
                    !r.IsDeleted, cancellationToken);
        }

        public async Task<List<ChatRoom>> GetChatRoomsForUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.ChatRooms
                .Include(r => r.Sender)
                .Include(r => r.Receiver)
                .Include(r => r.ChatMessages)
                .Where(r => (r.SenderId == userId || r.ReceiverId == userId) && !r.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task AddChatRoomAsync(ChatRoom chatRoom, CancellationToken cancellationToken)
        {
            await _context.ChatRooms.AddAsync(chatRoom, cancellationToken);
        }

        public async Task AddChatMessageAsync(ChatMessage message, CancellationToken cancellationToken)
        {
            await _context.ChatMessages.AddAsync(message, cancellationToken);
        }

        public async Task<List<ChatMessage>> GetMessagesForRoomAsync(Guid chatRoomId, CancellationToken cancellationToken)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.ChatRoomId == chatRoomId && !m.IsDeleted)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
