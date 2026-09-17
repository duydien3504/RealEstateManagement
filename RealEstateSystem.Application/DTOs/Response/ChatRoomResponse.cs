namespace RealEstateSystem.Application.DTOs.Response
{
    public class ChatRoomResponse
    {
        public Guid ChatRoomId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? SenderAvatar { get; set; }
        public Guid ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string? ReceiverAvatar { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
