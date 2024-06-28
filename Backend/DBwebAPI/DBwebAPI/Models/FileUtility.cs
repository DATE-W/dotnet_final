using System;
namespace DBwebAPI.Models
{
    //验证文件后缀名合法性
    public class FileUtility
    {
        public static bool IsFileExtensionValid(IFormFile file, string[] allowedExtensions)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
    }
}

