using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateSystem.Application.DTOs.Request
{
    public class VerifyOtpRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
