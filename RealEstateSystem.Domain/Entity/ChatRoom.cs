using RealEstateSystem.Domain.Common;

namespace RealEstateSystem.Domain.Entity
{
    public class ChatRoom : SoftDeleteEntity
    {
        public Guid ChatRoomId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;

        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}
