using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace TestConsole.Helper;

public class JWTHelper
{
    private static byte[] _key = Encoding.ASCII.GetBytes("QmFzZTY0RW5jb2RlZFJhbmRvbVNlY3JldEtleQ==");
    private static JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
    private static string? _xitmentSecretKey = "b7f6e3d9c4a2f18b6d8e5a9c7f1b2e3d4c6a7f8e9d2b3c4a5f6d7e8c9b1a2f3";

    public static string Encode<T>(T payload, TimeSpan expiredTime)
    {
        var encrypt = Encrypt(payload, _xitmentSecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Payload", encrypt)
            }),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Expires = DateTime.UtcNow + expiredTime
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public static T Decode<T>(string token)
    {
        _tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            SaveSigninToken = true,
            IssuerSigningKey = new SymmetricSecurityKey(_key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        var payload = ((JwtSecurityToken)validatedToken).Claims.First(x => x.Type == "Payload").Value;

        return Decrypt<T>(payload, _xitmentSecretKey);
    }

    private static string Encrypt<T>(T source, string key, bool isUrlEncode = false)
    {
        var serializeObject = SerializeTypedObject(source);
        var payload = Encrypt(serializeObject, key);
        return isUrlEncode ? HttpUtility.UrlEncode(payload) : payload;
    }
    private static string Encrypt(string source, string key)
    {
        var des = new DESCryptoServiceProvider();

        var rfc2898 = new Rfc2898DeriveBytes(key, HashByMD5(key));

        des.Key = rfc2898.GetBytes(des.KeySize / 8);
        des.IV = rfc2898.GetBytes(des.BlockSize / 8);

        var dateByteArray = Encoding.UTF8.GetBytes(source);

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(dateByteArray, 0, dateByteArray.Length);
        cs.FlushFinalBlock();

        return Convert.ToBase64String(ms.ToArray());
    }

    private static T Decrypt<T>(string payload, string key)
    {
        try
        {
            var decrypt = Decrypt(payload, key);
            return DeserializeTypedObject<T>(decrypt);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    private static string Decrypt(string encrypted, string key)
    {
        var des = new DESCryptoServiceProvider();

        var rfc2898 = new Rfc2898DeriveBytes(key, HashByMD5(key));

        des.Key = rfc2898.GetBytes(des.KeySize / 8);
        des.IV = rfc2898.GetBytes(des.BlockSize / 8);

        var dateByteArray = Convert.FromBase64String(encrypted);

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(dateByteArray, 0, dateByteArray.Length);
        cs.FlushFinalBlock();

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private static byte[] HashByMD5(string source)
    {
        return MD5.HashData(Encoding.UTF8.GetBytes(source));
    }

    private static string SerializeTypedObject(object obj)
    {
        var setting = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto,
        };
        var serializeObject = JsonConvert.SerializeObject(obj, setting);
        return serializeObject;
    }
    private static T DeserializeTypedObject<T>(string json)
    {
        var setting = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto,
        };
        return JsonConvert.DeserializeObject<T>(json, setting);
    }
}