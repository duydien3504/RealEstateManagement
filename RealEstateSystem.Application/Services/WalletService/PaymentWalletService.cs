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

namespace RealEstateSystem.Application.Services.WalletService
{
    public class PaymentWalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IPayment _paymentService;
        private readonly ILogger<PaymentWalletService> _logger;

        public PaymentWalletService(IWalletRepository walletRepository, IPayment paymentService, ILogger<PaymentWalletService> logger)
        {
            _walletRepository = walletRepository;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<TopUpResponse> TopUpWalletAsync(Guid userId, string email, TopUpRequest request, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _walletRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return new TopUpResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại."
                };
            }

            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                return new TopUpResponse
                {
                    IsSuccess = false,
                    Message = "Email không khớp với tài khoản."
                };
            }

            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId, cancellationToken);
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    WalletId = Guid.NewGuid(),
                    UserId = userId
                };
                await _walletRepository.AddWalletAsync(wallet, cancellationToken);
                await _walletRepository.SaveChangesAsync(cancellationToken);
            }

            var transactionCode = $"WT_{Guid.NewGuid().ToString("N").Substring(0, 16)}";
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                UserId = userId,
                WalletId = wallet.WalletId,
                Amount = request.Amount,
                TransactionCode = transactionCode,
                OrderInfo = "Nap tien vao vi",
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepository.AddTransactionAsync(transaction, cancellationToken);
            await _walletRepository.SaveChangesAsync(cancellationToken);

            var paymentUrl = _paymentService.CreatePaymentUrl(transactionCode, (double)request.Amount, "Nap tien vao vi", ipAddress);

            return new TopUpResponse
            {
                IsSuccess = true,
                Message = "Tạo liên kết nạp tiền thành công.",
                PaymentUrl = paymentUrl
            };
        }

        public async Task<VnpayReturnResponse> ProcessReturnUrlAsync(Dictionary<string, string> queryParameters, CancellationToken cancellationToken)
        {
            var isValidSignature = _paymentService.ValidateCallback(queryParameters);
            queryParameters.TryGetValue("vnp_TxnRef", out var transactionCode);
            queryParameters.TryGetValue("vnp_ResponseCode", out var responseCode);

            if (!isValidSignature)
            {
                _logger.LogWarning("Chữ ký VnPay không hợp lệ cho giao dịch ví {TransactionCode}.", transactionCode);
                return new VnpayReturnResponse
                {
                    IsSuccess = false,
                    Message = "Chữ ký xác thực không hợp lệ.",
                    TransactionCode = transactionCode ?? string.Empty,
                    ResponseCode = responseCode
                };
            }

            if (responseCode == "00")
            {
                return new VnpayReturnResponse
                {
                    IsSuccess = true,
                    Message = "Thanh toán nạp tiền thành công. Số tiền sẽ được cộng vào ví của bạn.",
                    TransactionCode = transactionCode ?? string.Empty,
                    ResponseCode = responseCode
                };
            }

            return new VnpayReturnResponse
            {
                IsSuccess = false,
                Message = "Thanh toán nạp tiền thất bại hoặc bị hủy bởi người dùng.",
                TransactionCode = transactionCode ?? string.Empty,
                ResponseCode = responseCode
            };
        }

        public async Task<VnpayIpnResponse> ProcessIpnAsync(Dictionary<string, string> queryParameters, CancellationToken cancellationToken)
        {
            if (!_paymentService.ValidateCallback(queryParameters))
            {
                _logger.LogWarning("IPN Ví: Chữ ký VnPay không hợp lệ.");
                return new VnpayIpnResponse { RspCode = "97", Message = "Chữ ký không hợp lệ" };
            }

            if (!queryParameters.TryGetValue("vnp_TxnRef", out var transactionCode))
            {
                return new VnpayIpnResponse { RspCode = "01", Message = "Không tìm thấy mã giao dịch" };
            }

            queryParameters.TryGetValue("vnp_ResponseCode", out var responseCode);
            queryParameters.TryGetValue("vnp_Amount", out var vnpAmountStr);

            var transaction = await _walletRepository.GetTransactionByCodeAsync(transactionCode, cancellationToken);
            if (transaction == null)
            {
                _logger.LogWarning("IPN Ví: Không tìm thấy giao dịch với mã {TransactionCode}.", transactionCode);
                return new VnpayIpnResponse { RspCode = "01", Message = "Không tìm thấy đơn hàng" };
            }

            if (long.TryParse(vnpAmountStr, out var vnpAmount))
            {
                var expectedAmount = (long)(transaction.Amount * 100);
                if (vnpAmount != expectedAmount)
                {
                    _logger.LogWarning("IPN Ví: Số tiền không khớp cho giao dịch {TransactionCode}.", transactionCode);
                    return new VnpayIpnResponse { RspCode = "04", Message = "Số tiền không hợp lệ" };
                }
            }

            if (transaction.PaymentStatus != PaymentStatus.Pending)
            {
                return new VnpayIpnResponse { RspCode = "02", Message = "Đơn hàng đã được xử lý" };
            }

            transaction.ResponseCode = responseCode;

            if (responseCode == "00")
            {
                await _walletRepository.BeginTransactionAsync(cancellationToken);
                try
                {
                    transaction.PaymentStatus = PaymentStatus.Success;
                    transaction.PaidAt = DateTime.UtcNow;

                    var walletTransaction = new WalletTransaction
                    {
                        WalletTransactionId = Guid.NewGuid(),
                        WalletId = transaction.WalletId!.Value,
                        Amount = transaction.Amount,
                        TransactionType = WalletTransactionType.TopUp,
                        ReferenceType = null,
                        ReferenceId = null,
                        Description = "Nap tien vao vi qua VnPay",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _walletRepository.AddWalletTransactionAsync(walletTransaction, cancellationToken);
                    await _walletRepository.SaveChangesAsync(cancellationToken);
                    await _walletRepository.CommitTransactionAsync(cancellationToken);

                    return new VnpayIpnResponse { RspCode = "00", Message = "Xác nhận thành công" };
                }
                catch (Exception ex)
                {
                    await _walletRepository.RollbackTransactionAsync(cancellationToken);
                    _logger.LogError(ex, "IPN Ví: Lỗi khi xử lý nạp tiền cho giao dịch {TransactionCode}.", transactionCode);
                    return new VnpayIpnResponse { RspCode = "99", Message = "Lỗi hệ thống" };
                }
            }
            else
            {
                transaction.PaymentStatus = PaymentStatus.Failed;
                await _walletRepository.SaveChangesAsync(cancellationToken);
                return new VnpayIpnResponse { RspCode = "00", Message = "Xác nhận thành công" };
            }
        }

        public async Task<WalletDetailsResponse> GetWalletDetailsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var user = await _walletRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new RealEstateSystem.Domain.Exceptions.NotFoundException("Người dùng không tồn tại.");
            }

            var result = await _walletRepository.GetWalletDetailsAsync(userId, pageNumber, pageSize, cancellationToken);
            if (result == null)
            {
                return new WalletDetailsResponse
                {
                    WalletId = Guid.Empty,
                    Balance = 0,
                    Transactions = new List<WalletTransactionDto>(),
                    TotalTransactions = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            return result;
        }
    }
}
