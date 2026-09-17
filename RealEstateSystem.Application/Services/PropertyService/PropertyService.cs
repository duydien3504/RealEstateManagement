using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Application.Services.PropertyService
{
    public class PropertyService : IPropertyService
    {
        private const decimal PropertyPostingFee = 50000m;

        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ILogger<PropertyService> _logger;

        public PropertyService(
            IUserRepository userRepository,
            IWalletRepository walletRepository,
            IPropertyRepository propertyRepository,
            ILogger<PropertyService> logger)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
            _propertyRepository = propertyRepository;
            _logger = logger;
        }

        public async Task<CreatePropertyResponse> CreatePropertyAsync(Guid userId, CreatePropertyRequest request, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
            {
                return new CreatePropertyResponse
                {
                    IsSuccess = false,
                    Message = "Yêu cầu xác thực không hợp lệ."
                };
            }

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return new CreatePropertyResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại."
                };
            }

            var propertyId = Guid.NewGuid();
            var property = new Property
            {
                PropertyId = propertyId,
                OwnerId = userId,
                CategoryId = request.CategoryId,
                WardId = request.WardId,
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                Area = request.Area,
                Dimensions = request.Dimensions,
                AddressDetail = request.AddressDetail,
                NumBedrooms = request.NumBedrooms,
                PropertyStatusValue = PropertyStatus.Available,
                DisplayStatusValue = DisplayStatus.Pending,
                IsPremium = false,
                ViewCount = 0,
                FavoriteCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            foreach (var amenityId in request.AmenityIds.Distinct())
            {
                property.PropertyAmenities.Add(new PropertyAmenity
                {
                    PropertyAmenityId = Guid.NewGuid(),
                    PropertyId = propertyId,
                    AmenityId = amenityId
                });
            }

            if (request.Medias != null)
            {
                foreach (var media in request.Medias)
                {
                    property.PropertyMedias.Add(new PropertyMedia
                    {
                        MediaId = Guid.NewGuid(),
                        PropertyId = propertyId,
                        MediaUrl = media.MediaUrl,
                        MediaType = Enum.Parse<MediaType>(media.MediaType, true),
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            var userRole = user.Role.NameRole;

            if (userRole == RoleType.Admin)
            {
                property.DisplayStatusValue = DisplayStatus.Approved;

                await _propertyRepository.AddAsync(property, cancellationToken);
                await _propertyRepository.SaveChangesAsync(cancellationToken);

                return new CreatePropertyResponse
                {
                    IsSuccess = true,
                    Message = "Đăng bài bất động sản thành công.",
                    PropertyId = propertyId
                };
            }

            if (userRole != RoleType.Owner)
            {
                return new CreatePropertyResponse
                {
                    IsSuccess = false,
                    Message = "Bạn không có quyền đăng bài bất động sản."
                };
            }

            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId, cancellationToken);
            if (wallet == null)
            {
                return new CreatePropertyResponse
                {
                    IsSuccess = false,
                    Message = "Ví không tồn tại. Vui lòng nạp tiền vào ví để đăng bài."
                };
            }

            var totalTopUp = wallet.WalletTransactions
                .Where(wt => wt.TransactionType == WalletTransactionType.TopUp || wt.TransactionType == WalletTransactionType.Refund)
                .Sum(wt => wt.Amount);

            var totalPayment = wallet.WalletTransactions
                .Where(wt => wt.TransactionType == WalletTransactionType.Payment)
                .Sum(wt => wt.Amount);

            var balance = totalTopUp - totalPayment;

            if (balance < PropertyPostingFee)
            {
                return new CreatePropertyResponse
                {
                    IsSuccess = false,
                    Message = $"Số dư ví không đủ. Yêu cầu tối thiểu {PropertyPostingFee:N0} VND để đăng bài."
                };
            }

            await _propertyRepository.BeginTransactionAsync(cancellationToken);
            try
            {
                await _propertyRepository.AddAsync(property, cancellationToken);

                var walletTransaction = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    WalletId = wallet.WalletId,
                    Amount = PropertyPostingFee,
                    TransactionType = WalletTransactionType.Payment,
                    ReferenceType = WalletReferenceType.PropertyPayment,
                    ReferenceId = propertyId.ToString(),
                    Description = "Thanh toan phi dang bai bat dong san",
                    CreatedAt = DateTime.UtcNow
                };
                await _walletRepository.AddWalletTransactionAsync(walletTransaction, cancellationToken);

                var propertyPayment = new PropertyPayment
                {
                    PropertyPaymentId = Guid.NewGuid(),
                    PropertyId = propertyId,
                    WalletTransactionId = walletTransaction.WalletTransactionId,
                    TransactionId = null,
                    ExpiredAt = DateTime.UtcNow.AddDays(30)
                };
                await _propertyRepository.AddPropertyPaymentAsync(propertyPayment, cancellationToken);

                await _propertyRepository.SaveChangesAsync(cancellationToken);
                await _propertyRepository.CommitTransactionAsync(cancellationToken);

                return new CreatePropertyResponse
                {
                    IsSuccess = true,
                    Message = "Đăng bài bất động sản thành công. Bài đăng đang chờ duyệt.",
                    PropertyId = propertyId
                };
            }
            catch (Exception ex)
            {
                await _propertyRepository.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Loi khi dang bai bat dong san cho user {UserId}.", userId);

                return new CreatePropertyResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống khi xử lý đăng bài bất động sản."
                };
            }
        }

        public async Task<List<PropertyResponseDto>> GetMyPropertiesAsync(Guid ownerId, CancellationToken cancellationToken)
        {
            var properties = await _propertyRepository.GetPropertiesByOwnerIdAsync(ownerId, cancellationToken);
            return properties.Select(MapToResponseDto).ToList();
        }

        public async Task<PropertyResponseDto?> GetPropertyDetailAsync(Guid propertyId, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null)
            {
                return null;
            }

            property.ViewCount++;
            await _propertyRepository.UpdateAsync(property, cancellationToken);
            await _propertyRepository.SaveChangesAsync(cancellationToken);

            return MapToResponseDto(property);
        }

        public async Task<PaginatedPropertiesResponse> GetPublicPropertiesAsync(GetPublicPropertiesRequest request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _propertyRepository.GetPublicPropertiesAsync(request, cancellationToken);
            var responseDtos = items.Select(MapToResponseDto).ToList();
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new PaginatedPropertiesResponse
            {
                Items = responseDtos,
                TotalItems = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<bool> UpdatePropertyAsync(Guid userId, Guid propertyId, UpdatePropertyRequest request, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null)
            {
                return false;
            }

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null || (property.OwnerId != userId && user.Role.NameRole != RoleType.Admin))
            {
                return false;
            }

            property.CategoryId = request.CategoryId;
            property.WardId = request.WardId;
            property.Title = request.Title;
            property.Description = request.Description;
            property.Price = request.Price;
            property.Area = request.Area;
            property.Dimensions = request.Dimensions;
            property.AddressDetail = request.AddressDetail;
            property.NumBedrooms = request.NumBedrooms;
            property.UpdatedAt = DateTime.UtcNow;

            // Sync Amenities
            var requestedAmenityIds = request.AmenityIds.Distinct().ToList();
            var existingAmenityIds = property.PropertyAmenities.Select(a => a.AmenityId).ToList();

            var amenitiesToRemove = property.PropertyAmenities.Where(a => !requestedAmenityIds.Contains(a.AmenityId)).ToList();
            foreach (var item in amenitiesToRemove)
            {
                property.PropertyAmenities.Remove(item);
            }

            var amenitiesToAdd = requestedAmenityIds.Except(existingAmenityIds).ToList();
            foreach (var id in amenitiesToAdd)
            {
                property.PropertyAmenities.Add(new PropertyAmenity
                {
                    PropertyId = propertyId,
                    AmenityId = id
                });
            }

            // Sync Medias
            var requestedMedias = request.Medias ?? new List<PropertyMediaRequest>();
            var existingMediaUrls = property.PropertyMedias.Select(m => m.MediaUrl).ToList();

            var mediasToRemove = property.PropertyMedias.Where(m => !requestedMedias.Any(rm => rm.MediaUrl == m.MediaUrl)).ToList();
            foreach (var item in mediasToRemove)
            {
                property.PropertyMedias.Remove(item);
            }

            var mediasToAdd = requestedMedias.Where(rm => !existingMediaUrls.Contains(rm.MediaUrl)).ToList();
            foreach (var rm in mediasToAdd)
            {
                property.PropertyMedias.Add(new PropertyMedia
                {
                    PropertyId = propertyId,
                    MediaUrl = rm.MediaUrl,
                    MediaType = Enum.Parse<MediaType>(rm.MediaType, true),
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _propertyRepository.UpdateAsync(property, cancellationToken);
            await _propertyRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> SoftDeletePropertyAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null)
            {
                return false;
            }

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null || (property.OwnerId != userId && user.Role.NameRole != RoleType.Admin))
            {
                return false;
            }

            property.IsDeleted = true;
            property.UpdatedAt = DateTime.UtcNow;

            await _propertyRepository.UpdateAsync(property, cancellationToken);
            await _propertyRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ApprovePropertyAsync(Guid propertyId, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null || property.IsDeleted)
            {
                return false;
            }

            property.DisplayStatusValue = DisplayStatus.Approved;
            var payment = property.PropertyPayments.OrderByDescending(p => p.ExpiredAt).FirstOrDefault();
            property.ExpiredAt = payment?.ExpiredAt ?? DateTime.UtcNow.AddDays(30);
            property.UpdatedAt = DateTime.UtcNow;

            await _propertyRepository.UpdateAsync(property, cancellationToken);
            await _propertyRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RejectPropertyAsync(Guid propertyId, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null || property.IsDeleted)
            {
                return false;
            }

            property.DisplayStatusValue = DisplayStatus.Rejected;
            property.UpdatedAt = DateTime.UtcNow;

            await _propertyRepository.UpdateAsync(property, cancellationToken);
            await _propertyRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<PropertyActionResponse> ExtendPropertyAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null)
            {
                return new PropertyActionResponse { IsSuccess = false, Message = "Tin đăng không tồn tại." };
            }

            if (property.OwnerId != userId)
            {
                return new PropertyActionResponse { IsSuccess = false, Message = "Bạn không có quyền gia hạn tin đăng này." };
            }

            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId, cancellationToken);
            if (wallet == null)
            {
                return new PropertyActionResponse { IsSuccess = false, Message = "Ví không tồn tại. Vui lòng nạp tiền." };
            }

            var totalTopUp = wallet.WalletTransactions
                .Where(wt => wt.TransactionType == WalletTransactionType.TopUp || wt.TransactionType == WalletTransactionType.Refund)
                .Sum(wt => wt.Amount);

            var totalPayment = wallet.WalletTransactions
                .Where(wt => wt.TransactionType == WalletTransactionType.Payment)
                .Sum(wt => wt.Amount);

            var balance = totalTopUp - totalPayment;

            if (balance < PropertyPostingFee)
            {
                return new PropertyActionResponse { IsSuccess = false, Message = $"Số dư ví không đủ. Yêu cầu tối thiểu {PropertyPostingFee:N0} VND để gia hạn." };
            }

            await _propertyRepository.BeginTransactionAsync(cancellationToken);
            try
            {
                var walletTransaction = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    WalletId = wallet.WalletId,
                    Amount = PropertyPostingFee,
                    TransactionType = WalletTransactionType.Payment,
                    ReferenceType = WalletReferenceType.PropertyPayment,
                    ReferenceId = propertyId.ToString(),
                    Description = "Gia han tin dang bat dong san",
                    CreatedAt = DateTime.UtcNow
                };
                await _walletRepository.AddWalletTransactionAsync(walletTransaction, cancellationToken);

                var propertyPayment = new PropertyPayment
                {
                    PropertyPaymentId = Guid.NewGuid(),
                    PropertyId = propertyId,
                    WalletTransactionId = walletTransaction.WalletTransactionId,
                    TransactionId = null,
                    ExpiredAt = (property.ExpiredAt > DateTime.UtcNow ? property.ExpiredAt.Value : DateTime.UtcNow).AddDays(30)
                };
                await _propertyRepository.AddPropertyPaymentAsync(propertyPayment, cancellationToken);

                property.ExpiredAt = propertyPayment.ExpiredAt;
                property.UpdatedAt = DateTime.UtcNow;

                await _propertyRepository.UpdateAsync(property, cancellationToken);
                await _propertyRepository.SaveChangesAsync(cancellationToken);
                await _propertyRepository.CommitTransactionAsync(cancellationToken);

                return new PropertyActionResponse { IsSuccess = true, Message = "Gia hạn tin đăng thành công." };
            }
            catch (Exception ex)
            {
                await _propertyRepository.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Lỗi khi gia hạn tin đăng {PropertyId}.", propertyId);
                return new PropertyActionResponse { IsSuccess = false, Message = "Lỗi hệ thống khi gia hạn tin đăng." };
            }
        }

        public async Task<PropertyActionResponse> UpgradeToPremiumAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null)
            {
                return new PropertyActionResponse { IsSuccess = false, Message = "Tin đăng không tồn tại." };
            }

            if (property.OwnerId != userId)
            {
                return new PropertyActionResponse { IsSuccess = false, Message = "Bạn không có quyền đăng ký Premium cho tin đăng này." };
            }

            if (property.IsPremium)
            {
                return new PropertyActionResponse { IsSuccess = false, Message = "Tin đăng đã ở trạng thái Premium." };
            }

            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId, cancellationToken);
            if (wallet == null)
            {
                return new PropertyActionResponse { IsSuccess = false, Message = "Ví không tồn tại. Vui lòng nạp tiền." };
            }

            var totalTopUp = wallet.WalletTransactions
                .Where(wt => wt.TransactionType == WalletTransactionType.TopUp || wt.TransactionType == WalletTransactionType.Refund)
                .Sum(wt => wt.Amount);

            var totalPayment = wallet.WalletTransactions
                .Where(wt => wt.TransactionType == WalletTransactionType.Payment)
                .Sum(wt => wt.Amount);

            var balance = totalTopUp - totalPayment;

            if (balance < PropertyPostingFee)
            {
                return new PropertyActionResponse { IsSuccess = false, Message = $"Số dư ví không đủ. Yêu cầu tối thiểu {PropertyPostingFee:N0} VND để đăng ký Premium." };
            }

            await _propertyRepository.BeginTransactionAsync(cancellationToken);
            try
            {
                var walletTransaction = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    WalletId = wallet.WalletId,
                    Amount = PropertyPostingFee,
                    TransactionType = WalletTransactionType.Payment,
                    ReferenceType = WalletReferenceType.PropertyPayment,
                    ReferenceId = propertyId.ToString(),
                    Description = "Dang ky tin dang Premium",
                    CreatedAt = DateTime.UtcNow
                };
                await _walletRepository.AddWalletTransactionAsync(walletTransaction, cancellationToken);

                var propertyPayment = new PropertyPayment
                {
                    PropertyPaymentId = Guid.NewGuid(),
                    PropertyId = propertyId,
                    WalletTransactionId = walletTransaction.WalletTransactionId,
                    TransactionId = null,
                    ExpiredAt = (property.ExpiredAt > DateTime.UtcNow ? property.ExpiredAt.Value : DateTime.UtcNow).AddDays(30)
                };
                await _propertyRepository.AddPropertyPaymentAsync(propertyPayment, cancellationToken);

                property.IsPremium = true;
                property.ExpiredAt = propertyPayment.ExpiredAt;
                property.UpdatedAt = DateTime.UtcNow;

                await _propertyRepository.UpdateAsync(property, cancellationToken);
                await _propertyRepository.SaveChangesAsync(cancellationToken);
                await _propertyRepository.CommitTransactionAsync(cancellationToken);

                return new PropertyActionResponse { IsSuccess = true, Message = "Đăng ký Premium cho tin đăng thành công." };
            }
            catch (Exception ex)
            {
                await _propertyRepository.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Lỗi khi đăng ký Premium cho tin đăng {PropertyId}.", propertyId);
                return new PropertyActionResponse { IsSuccess = false, Message = "Lỗi hệ thống khi đăng ký Premium." };
            }
        }

        private PropertyResponseDto MapToResponseDto(Property property)
        {
            return new PropertyResponseDto
            {
                PropertyId = property.PropertyId,
                OwnerId = property.OwnerId,
                CategoryId = property.CategoryId,
                CategoryName = property.Category?.Name ?? string.Empty,
                WardId = property.WardId,
                WardName = property.Ward?.Name ?? string.Empty,
                ProvinceName = property.Ward?.Province?.Name ?? string.Empty,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                Area = property.Area,
                Dimensions = property.Dimensions,
                AddressDetail = property.AddressDetail,
                NumBedrooms = property.NumBedrooms,
                PropertyStatus = property.PropertyStatusValue.ToString(),
                DisplayStatus = property.DisplayStatusValue.ToString(),
                IsPremium = property.IsPremium,
                ExpiredAt = property.ExpiredAt,
                ViewCount = property.ViewCount,
                FavoriteCount = property.FavoriteCount,
                CreatedAt = property.CreatedAt,
                Medias = property.PropertyMedias.Select(m => new PropertyMediaResponseDto
                {
                    MediaId = m.MediaId,
                    MediaUrl = m.MediaUrl,
                    MediaType = m.MediaType.ToString()
                }).ToList(),
                Amenities = property.PropertyAmenities.Select(pa => new AmenityResponseDto
                {
                    AmenityId = pa.AmenityId,
                    Name = pa.Amenity?.Name ?? string.Empty,
                    IconUrl = pa.Amenity?.IconUrl
                }).ToList()
            };
        }
    }
}
