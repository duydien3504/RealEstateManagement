using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public ChatService(IChatRepository chatRepository, IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<ChatRoomResponse> CreateChatRoomAsync(Guid senderId, Guid receiverId, CancellationToken cancellationToken)
        {
            var sender = await _userRepository.GetUserByIdAsync(senderId, cancellationToken);
            var receiver = await _userRepository.GetUserByIdAsync(receiverId, cancellationToken);

            if (sender == null || receiver == null)
            {
                throw new NotFoundException("Người dùng không tồn tại.");
            }

            var existingRoom = await _chatRepository.GetChatRoomByParticipantsAsync(senderId, receiverId, cancellationToken);
            if (existingRoom != null)
            {
                return MapToChatRoomResponse(existingRoom, senderId);
            }

            var newRoom = new ChatRoom
            {
                ChatRoomId = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId
            };

            await _chatRepository.AddChatRoomAsync(newRoom, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            var createdRoom = await _chatRepository.GetChatRoomByIdAsync(newRoom.ChatRoomId, cancellationToken);
            if (createdRoom == null)
            {
                throw new NotFoundException("Không thể khởi tạo phòng chat.");
            }

            return MapToChatRoomResponse(createdRoom, senderId);
        }

        public async Task<List<ChatRoomResponse>> GetChatRoomsForUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var rooms = await _chatRepository.GetChatRoomsForUserAsync(userId, cancellationToken);
            var responseList = new List<ChatRoomResponse>();

            foreach (var room in rooms)
            {
                responseList.Add(MapToChatRoomResponse(room, userId));
            }

            return responseList.OrderByDescending(r => r.LastMessageTime).ToList();
        }

        public async Task<List<ChatMessageResponse>> GetMessagesAsync(Guid chatRoomId, Guid userId, CancellationToken cancellationToken)
        {
            var room = await _chatRepository.GetChatRoomByIdAsync(chatRoomId, cancellationToken);
            if (room == null)
            {
                throw new NotFoundException("Phòng chat không tồn tại.");
            }

            if (room.SenderId != userId && room.ReceiverId != userId)
            {
                throw new UnauthorizedException("Bạn không có quyền truy cập phòng chat này.");
            }

            var messages = await _chatRepository.GetMessagesForRoomAsync(chatRoomId, cancellationToken);

            var unreadMessages = messages.Where(m => m.SenderId != userId && !m.IsRead).ToList();
            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                }
                await _chatRepository.SaveChangesAsync(cancellationToken);
            }

            return messages.Select(m => new ChatMessageResponse
            {
                ChatMessageId = m.ChatMessageId,
                ChatRoomId = m.ChatRoomId,
                SenderId = m.SenderId,
                SenderName = m.Sender.FullName,
                Message = m.Message,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead
            }).OrderBy(m => m.CreatedAt).ToList();
        }

        public async Task<ChatMessageResponse> SendMessageAsync(Guid senderId, Guid chatRoomId, string messageContent, CancellationToken cancellationToken)
        {
            var room = await _chatRepository.GetChatRoomByIdAsync(chatRoomId, cancellationToken);
            if (room == null)
            {
                throw new NotFoundException("Phòng chat không tồn tại.");
            }

            if (room.SenderId != senderId && room.ReceiverId != senderId)
            {
                throw new UnauthorizedException("Bạn không có quyền gửi tin nhắn trong phòng chat này.");
            }

            var message = new ChatMessage
            {
                ChatMessageId = Guid.NewGuid(),
                ChatRoomId = chatRoomId,
                SenderId = senderId,
                Message = messageContent,
                IsRead = false
            };

            await _chatRepository.AddChatMessageAsync(message, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            var sender = await _userRepository.GetUserByIdAsync(senderId, cancellationToken);

            return new ChatMessageResponse
            {
                ChatMessageId = message.ChatMessageId,
                ChatRoomId = message.ChatRoomId,
                SenderId = message.SenderId,
                SenderName = sender?.FullName ?? string.Empty,
                Message = message.Message,
                CreatedAt = message.CreatedAt,
                IsRead = message.IsRead
            };
        }

        private static ChatRoomResponse MapToChatRoomResponse(ChatRoom room, Guid userId)
        {
            var lastMessageObj = room.ChatMessages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
            var unreadCount = room.ChatMessages.Count(m => m.SenderId != userId && !m.IsRead);

            return new ChatRoomResponse
            {
                ChatRoomId = room.ChatRoomId,
                SenderId = room.SenderId,
                SenderName = room.Sender.FullName,
                SenderAvatar = room.Sender.AvatarUrl,
                ReceiverId = room.ReceiverId,
                ReceiverName = room.Receiver.FullName,
                ReceiverAvatar = room.Receiver.AvatarUrl,
                LastMessage = lastMessageObj?.Message ?? string.Empty,
                LastMessageTime = lastMessageObj?.CreatedAt ?? room.CreatedAt,
                UnreadCount = unreadCount
            };
        }
    }
}
