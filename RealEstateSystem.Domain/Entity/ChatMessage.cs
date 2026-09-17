using RealEstateSystem.Domain.Common;

namespace RealEstateSystem.Domain.Entity
{
    public class ChatMessage : SoftDeleteEntity
    {
        public Guid ChatMessageId { get; set; }
        public Guid ChatRoomId { get; set; }
        public Guid SenderId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }

        public ChatRoom ChatRoom { get; set; } = null!;
        public User Sender { get; set; } = null!;
    }
}
