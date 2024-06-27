using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using SqlSugar;
using System.Linq.Expressions;
using DBwebAPI;

namespace DBwebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Picture : ControllerBase
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
        [HttpPost]
        public async Task<IActionResult> SaveFile(List<IFormFile> files)
        {
            Console.WriteLine("--------------------------SaveFile--------------------------");
            string[] allowedExtensions = { ".jpg", ".png" };
            List<string> ret = new List<string>();
            try
            {
                int x = 0;
                foreach (IFormFile file in files)
                {
                    x++;
                    if (file == null || file.Length == 0)
                    {
                        Console.WriteLine("未接收到有效文件");
                        throw new Exception($"第{x}份文件无效");
                    }
                    else
                    {
                        Stream stream = file.OpenReadStream();
                        MD5 md5 = MD5.Create();
                        SHA256 sha256 = SHA256.Create();
                        string fileName;
                        var hash = sha256.ComputeHash(stream);
                        if (hash == null)
                        {
                            Console.WriteLine("哈希错了，怪");
                            return BadRequest(new CustomResponse { ok = "no", value = "服务器内部错误" });
                        }
                        else
                        {
                            // 将哈希值转换为16进制字符串
                            StringBuilder strb = new StringBuilder();
                            foreach (byte b in hash)
                            {
                                strb.Append(b.ToString("x2")); // 使用"x2"格式化将字节转换为16进制
                            }
                            fileName = strb.ToString();
                        }
                        // string filePath = @"C:\Users\13293\Desktop\";
                        string basePath = "/home/ubuntu/DataBase/";
                        string relPath = "pictures/";
                        string filePath = basePath + relPath;
                        string extension = Path.GetExtension(file.FileName);
                        filePath += fileName + extension;
                        using (Stream saveStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(saveStream);
                        }
                        // 在这里关闭 saveStream
                        Console.WriteLine("成功");
                        ret.Add(relPath + fileName + extension);
                    }
                }
                return Ok(new CustomResponse { ok = "yes", value = ret });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "上传失败！" });
            }
        }
    }
}
