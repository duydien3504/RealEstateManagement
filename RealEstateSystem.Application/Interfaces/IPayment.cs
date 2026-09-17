using System.Collections.Generic;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IPayment
    {
        string CreatePaymentUrl(string transactionCode, double amount, string orderInfo, string ipAddress);
        bool ValidateCallback(Dictionary<string, string> queryParameters);
    }
}
