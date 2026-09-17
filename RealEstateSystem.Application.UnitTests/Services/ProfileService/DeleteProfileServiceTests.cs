using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Application.Services.ProfileService;
using RealEstateSystem.Application.UnitTests.Common;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.UnitTests.Services.ProfileService
{
    public class DeleteProfileServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptEmail _encryptEmail;
        private readonly ILogger<DeleteProfileService> _logger;
        private readonly DeleteProfileService _deleteProfileService;

        public DeleteProfileServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _encryptEmail = Substitute.For<IEncryptEmail>();
            _logger = Substitute.For<ILogger<DeleteProfileService>>();
            _deleteProfileService = new DeleteProfileService(_userRepository, _encryptEmail, _logger);
        }

        [Fact]
        public async Task DeleteProfileAsync_ValidRequest_SoftDeletesUserAndReturnsResponse()
        {
            var userId = Guid.NewGuid();
            var email = "delete@example.com";
            var user = TestDataBuilder.CreateActiveUser(userId, email, "Active User", "hash");
            var request = new DeleteProfileRequest { Email = email };
            var encryptedEmail = "encrypted_email_string";

            _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
            _encryptEmail.Encrypt(email).Returns(encryptedEmail);

            var response = await _deleteProfileService.DeleteProfileAsync(userId, request, CancellationToken.None);

            response.Should().NotBeNull();
            response.Message.Should().Be("Yêu cầu xóa tài khoản đã được xử lý thành công.");
            response.EncryptedEmail.Should().Be(encryptedEmail);
            user.IsDeleted.Should().BeTrue();

            await _userRepository.Received(1).UpdateUserAsync(user, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            _encryptEmail.Received(1).Encrypt(email);
        }

        [Fact]
        public async Task DeleteProfileAsync_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var request = new DeleteProfileRequest { Email = "any@example.com" };

            _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

            var action = () => _deleteProfileService.DeleteProfileAsync(userId, request, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Tài khoản không tồn tại.");
        }

        [Fact]
        public async Task DeleteProfileAsync_EmailMismatch_ThrowsBadRequestException()
        {
            var userId = Guid.NewGuid();
            var email = "db_email@example.com";
            var user = TestDataBuilder.CreateActiveUser(userId, email, "Active User", "hash");
            var request = new DeleteProfileRequest { Email = "input_email@example.com" };

            _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

            var action = () => _deleteProfileService.DeleteProfileAsync(userId, request, CancellationToken.None);

            await action.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Email không khớp với tài khoản đang đăng nhập.");
        }
    }
}
