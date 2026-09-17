using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.FavoriteService
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IPropertyRepository _propertyRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository, IPropertyRepository propertyRepository)
        {
            _favoriteRepository = favoriteRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<ToggleFavoriteResponse> ToggleFavoriteAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null || property.IsDeleted)
            {
                throw new NotFoundException("Không tìm thấy tin đăng bất động sản.");
            }

            var existingFavorite = await _favoriteRepository.GetFavoriteAsync(userId, propertyId, cancellationToken);
            if (existingFavorite != null)
            {
                await _favoriteRepository.RemoveFavoriteAsync(existingFavorite, cancellationToken);
                
                property.FavoriteCount = Math.Max(0, property.FavoriteCount - 1);
                await _propertyRepository.UpdateAsync(property, cancellationToken);
                
                await _propertyRepository.SaveChangesAsync(cancellationToken);

                return new ToggleFavoriteResponse
                {
                    IsFavorite = false,
                    Message = "Đã bỏ lưu tin đăng bất động sản thành công."
                };
            }
            else
            {
                var favorite = new UserFavorite
                {
                    FavoriteId = Guid.NewGuid(),
                    UserId = userId,
                    PropertyId = propertyId,
                    CreatedAt = DateTime.UtcNow
                };

                await _favoriteRepository.AddFavoriteAsync(favorite, cancellationToken);

                property.FavoriteCount++;
                await _propertyRepository.UpdateAsync(property, cancellationToken);

                await _propertyRepository.SaveChangesAsync(cancellationToken);

                return new ToggleFavoriteResponse
                {
                    IsFavorite = true,
                    Message = "Đã lưu tin đăng bất động sản thành công."
                };
            }
        }

        public async Task<bool> UnfavoriteAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null || property.IsDeleted)
            {
                throw new NotFoundException("Không tìm thấy tin đăng bất động sản.");
            }

            var existingFavorite = await _favoriteRepository.GetFavoriteAsync(userId, propertyId, cancellationToken);
            if (existingFavorite == null)
            {
                return false;
            }

            await _favoriteRepository.RemoveFavoriteAsync(existingFavorite, cancellationToken);

            property.FavoriteCount = Math.Max(0, property.FavoriteCount - 1);
            await _propertyRepository.UpdateAsync(property, cancellationToken);

            await _propertyRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<PropertyResponseDto>> GetFavoritePropertiesAsync(Guid userId, CancellationToken cancellationToken)
        {
            var properties = await _favoriteRepository.GetFavoritePropertiesByUserIdAsync(userId, cancellationToken);
            return properties.Select(MapToResponseDto).ToList();
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
