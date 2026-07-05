using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Infrastructure.Security
{
    public class HmacEncrypt : IEncryptEmail
    {
        private readonly byte[] _secretKey;

        public HmacEncrypt(IConfiguration configuration)
        {
            var hmacSecretKey = configuration["Hmac:SecretKey"]
                ?? throw new InvalidOperationException("Cấu hình Hmac:SecretKey không tồn tại.");
            _secretKey = Encoding.UTF8.GetBytes(hmacSecretKey);
        }

        public string Encrypt(string email)
        {
            using var hmac = new HMACSHA256(_secretKey);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(email));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
