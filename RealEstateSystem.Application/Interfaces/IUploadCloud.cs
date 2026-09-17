using System.Threading;
using System.Threading.Tasks;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IUploadCloud
    {
        Task<string> UploadImageAsync(byte[] fileBytes, string fileName, string folder, CancellationToken cancellationToken);
        Task<string> UploadVideoAsync(byte[] fileBytes, string fileName, string folder, CancellationToken cancellationToken);
    }
}
