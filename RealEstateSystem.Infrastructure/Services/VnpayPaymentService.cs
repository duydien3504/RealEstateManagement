using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Infrastructure.Services
{
    public class VnpayPaymentService : IPayment
    {
        private readonly IConfiguration _configuration;

        public VnpayPaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(string transactionCode, double amount, string orderInfo, string ipAddress)
        {
            var tmnCode = _configuration["VnPay:vnp_TmnCode"];
            var hashSecret = _configuration["VnPay:vnp_HashSecret"];
            var vnpUrl = _configuration["VnPay:vnp_Url"];
            var returnUrl = _configuration["VnPay:ReturnUrl"];
            var version = _configuration["VnPay:Version"] ?? "2.1.0";
            var command = _configuration["VnPay:Command"] ?? "pay";
            var currCode = _configuration["VnPay:CurrCode"] ?? "VND";
            var locale = _configuration["VnPay:Locale"] ?? "vn";

            if (string.IsNullOrEmpty(tmnCode) || string.IsNullOrEmpty(hashSecret) || string.IsNullOrEmpty(vnpUrl) || string.IsNullOrEmpty(returnUrl))
            {
                throw new InvalidOperationException("Cấu hình VnPay không hợp lệ.");
            }

            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Version", version },
                { "vnp_Command", command },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", ((long)(amount * 100)).ToString(CultureInfo.InvariantCulture) },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", currCode },
                { "vnp_IpAddr", ipAddress },
                { "vnp_Locale", locale },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_TxnRef", transactionCode }
            };

            var data = new StringBuilder();
            foreach (var kv in vnpParams)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            var rawData = data.ToString();
            if (rawData.Length > 0)
            {
                rawData = rawData.Remove(rawData.Length - 1, 1);
            }

            var secureHash = HmacSha512(hashSecret, rawData);
            
            return $"{vnpUrl}?{rawData}&vnp_SecureHash={secureHash}";
        }

        public bool ValidateCallback(Dictionary<string, string> queryParameters)
        {
            var hashSecret = _configuration["VnPay:vnp_HashSecret"];
            if (string.IsNullOrEmpty(hashSecret))
            {
                throw new InvalidOperationException("Cấu hình vnp_HashSecret không hợp lệ.");
            }
            
            if (!queryParameters.TryGetValue("vnp_SecureHash", out var secureHash))
            {
                return false;
            }

            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var kv in queryParameters)
            {
                if (kv.Key.StartsWith("vnp_", StringComparison.Ordinal) && kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                {
                    vnpParams.Add(kv.Key, kv.Value);
                }
            }

            var data = new StringBuilder();
            foreach (var kv in vnpParams)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            var rawData = data.ToString();
            if (rawData.Length > 0)
            {
                rawData = rawData.Remove(rawData.Length - 1, 1);
            }

            var calculatedHash = HmacSha512(hashSecret, rawData);

            return string.Equals(calculatedHash, secureHash, StringComparison.OrdinalIgnoreCase);
        }

        private static string HmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            var hash = new StringBuilder();
            foreach (var b in hashBytes)
            {
                hash.Append(b.ToString("X2"));
            }
            return hash.ToString();
        }
    }
}
