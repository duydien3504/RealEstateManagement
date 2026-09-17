using System;
using System.IO;
using System.Linq;

namespace RealEstateSystem.Application.Common.Helpers
{
    public static class MediaHelper
    {
        public static bool IsValidImageSignature(byte[] fileBytes, string extension)
        {
            if (fileBytes == null || fileBytes.Length < 12)
            {
                return false;
            }

            var ext = extension.ToLowerInvariant();

            if (ext == ".jpg" || ext == ".jpeg")
            {
                return fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 && fileBytes[2] == 0xFF;
            }

            if (ext == ".webp")
            {
                var isRiff = fileBytes[0] == 0x52 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x46;
                var isWebp = fileBytes[8] == 0x57 && fileBytes[9] == 0x45 && fileBytes[10] == 0x42 && fileBytes[11] == 0x50;
                return isRiff && isWebp;
            }

            if (ext == ".html" || ext == ".htm")
            {
                return fileBytes.Length > 0 && fileBytes[0] == 0x3C;
            }

            return false;
        }

        public static bool IsValidVideoSignature(byte[] fileBytes, string extension)
        {
            if (fileBytes == null || fileBytes.Length < 8)
            {
                return false;
            }

            var ext = extension.ToLowerInvariant();

            if (ext == ".mp4")
            {
                return fileBytes[4] == 0x66 && fileBytes[5] == 0x74 && fileBytes[6] == 0x79 && fileBytes[7] == 0x70;
            }

            return false;
        }
    }
}
