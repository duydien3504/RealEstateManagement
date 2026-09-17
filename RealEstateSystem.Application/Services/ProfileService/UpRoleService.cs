using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Application.Services.ProfileService
{
    public class UpRoleService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<UpRoleService> _logger;

        public UpRoleService(IUserRepository userRepository, IWalletRepository walletRepository, ILogger<UpRoleService> logger)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
            _logger = logger;
        }

        public async Task<UpRoleResponse> RegisterUpRoleOwnerAsync(Guid userId, UpRoleRequest request, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return new UpRoleResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại."
                };
            }

            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                return new UpRoleResponse
                {
                    IsSuccess = false,
                    Message = "Email không khớp với tài khoản."
                };
            }

            if (user.Role.NameRole == RoleType.Owner)
            {
                return new UpRoleResponse
                {
                    IsSuccess = false,
                    Message = "Tài khoản đã là Owner."
                };
            }

            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId, cancellationToken);
            if (wallet == null)
            {
                return new UpRoleResponse
                {
                    IsSuccess = false,
                    Message = "Số dư tài khoản không đủ."
                };
            }

            var totalTopUp = wallet.WalletTransactions
                .Where(wt => wt.TransactionType == WalletTransactionType.TopUp || wt.TransactionType == WalletTransactionType.Refund)
                .Sum(wt => wt.Amount);
            var totalPayment = wallet.WalletTransactions
                .Where(wt => wt.TransactionType == WalletTransactionType.Payment)
                .Sum(wt => wt.Amount);
            var balance = totalTopUp - totalPayment;

            if (balance < 300000)
            {
                return new UpRoleResponse
                {
                    IsSuccess = false,
                    Message = "Số dư tài khoản không đủ."
                };
            }

            await _walletRepository.BeginTransactionAsync(cancellationToken);
            try
            {
                var latestRequest = await _userRepository.GetLatestPendingOwnerProfileRequestAsync(userId, cancellationToken);
                if (latestRequest == null)
                {
                    latestRequest = new OwnerProfileRequest
                    {
                        RequestId = Guid.NewGuid(),
                        UserId = userId,
                        IdCardNumber = request.IdCardNumber,
                        RawDocumentsUrl = request.RawDocumentsUrl,
                        Status = OwnerProfileRequestStatus.Approved
                    };
                    await _userRepository.AddOwnerProfileRequestAsync(latestRequest, cancellationToken);
                }
                else
                {
                    latestRequest.Status = OwnerProfileRequestStatus.Approved;
                }
                await _userRepository.SaveChangesAsync(cancellationToken);

                var walletTx = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    WalletId = wallet.WalletId,
                    Amount = 300000,
                    TransactionType = WalletTransactionType.Payment,
                    ReferenceType = WalletReferenceType.OwnerUpgrade,
                    ReferenceId = latestRequest.RequestId.ToString(),
                    Description = "Thanh toan nang cap tai khoan Owner bang vi",
                    CreatedAt = DateTime.UtcNow
                };
                await _walletRepository.AddWalletTransactionAsync(walletTx, cancellationToken);

                var upgradePayment = new OwnerUpgradePayment
                {
                    OwnerUpgradePaymentId = Guid.NewGuid(),
                    TransactionId = null,
                    OwnerProfileRequestId = latestRequest.RequestId,
                    WalletTransactionId = walletTx.WalletTransactionId
                };
                await _userRepository.AddOwnerUpgradePaymentAsync(upgradePayment, cancellationToken);

                var ownerRoleId = await _userRepository.GetRoleIdByRoleTypeAsync(RoleType.Owner, cancellationToken);
                if (ownerRoleId.HasValue)
                {
                    user.RoleId = ownerRoleId.Value;
                    await _userRepository.UpdateUserAsync(user, cancellationToken);
                }

                await _walletRepository.SaveChangesAsync(cancellationToken);
                await _walletRepository.CommitTransactionAsync(cancellationToken);

                return new UpRoleResponse
                {
                    IsSuccess = true,
                    Message = "Nâng cấp tài khoản Owner thành công."
                };
            }
            catch (Exception ex)
            {
                await _walletRepository.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Loi khi nang cap tai khoan bang vi cho user {UserId}.", userId);
                return new UpRoleResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống khi xử lý nâng cấp tài khoản."
                };
            }
        }
    }
}
