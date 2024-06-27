using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using NetTaste;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DBwebAPI
{
    public class CreateToken
    {
        private string key = "f47b558d-7654-458c-99f2-13b190ef0199";//密钥
        public string createToken(string account, string authority)
        {
            DateTime utcNow = DateTime.UtcNow;
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            
            // authority用于鉴定用户权限，应该保证是这两个值中的一个
            if(authority != "Normal" && authority != "Admin")
            {
                throw new Exception($"此权限不存在：{authority}");
            }
            // token中的claims用于储存自定义信息，如登录之后的用户id等
            // claim部分仅使用base64加密，base64是一个可逆的算法，因而不应在claim中加入敏感信息
            var claims = new[]
            {
                new Claim("account", account),
                // new Claim("password", password), 
                new Claim("Authority", authority),
            };
            JwtSecurityToken jwtToken = new JwtSecurityToken(
                            issuer: "https://110.40.206.206",//说实话，不知道这俩有啥用
                            audience: "IDK",//说实话，真不知道这个有啥用
                            claims: claims,
                            notBefore: utcNow,//生效时间，感觉也没用，但还是加了
                            expires: utcNow.AddHours(24),//过期时间
                            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)//规定加密方法，以及密钥
                            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);//生成token
            return token;
        }
    }
    public class ValidateToken
    {
        private string secretKey = "f47b558d-7654-458c-99f2-13b190ef0199";//密钥，同上
        public bool ValidateJwtToken(string token, ValidTokenAuthority option)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            bool valid = tokenHandler.CanReadToken(token);
            if (!valid)
            {
                //LogHelper.LogInformation("token格式验证失败");
                throw (new Exception("错误的token格式"));
            }
            var tokenS = tokenHandler.ReadJwtToken(token);
            var autret = tokenS.Claims.FirstOrDefault(o => o.Type == "Authority");
            string? aut = autret == null ? null : autret.Value;
            
            if(aut == null)
            {
                //无论如何，aut不应该为null，因为token中必须包含Authority值
                throw (new Exception("token的authority值错误，createToken.cs:62"));
            }
            switch(option)
            {
                case ValidTokenAuthority.Normal:
                    if(aut != "Normal")
                    {
                        throw (new Exception($"指定的身份为普通用户，但实际验证得到的是{aut}"));
                    }
                    break;
                case ValidTokenAuthority.Admin:
                    if (aut != "Admin")
                    {
                        throw (new Exception($"指定的身份为管理员，但实际验证得到的是{aut}"));
                    }
                    break;
                case ValidTokenAuthority.None:
                    break;
            }
        
            string alg = tokenS.Header.Alg;//读取出算法
            IList<string> aud = tokenS.Payload.Aud;
            string iss = tokenS.Payload.Iss;

            // 设置 Token 验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true, // 验证签发者
                ValidateAudience = true, // 验证受众
                ValidateLifetime = true,
                ValidAlgorithms = new[] {alg},
                ValidAudiences = aud,
                ValidIssuer = iss,
            };

            try
            {
                // 验证 Token
                tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // 验证失败
                return false;
            }
        }
    }
}
