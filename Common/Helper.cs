using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using FormHTML.Models;
using Microsoft.IdentityModel.Tokens;

namespace FormHTML.Common
{
    public static class Helper
    {
        public static JwtSecurityToken DecodeJwtToken(string jwtToken)
        {
            JwtSecurityToken decodedToken = null;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                if (jwtToken.Contains("Bearer"))
                    jwtToken = jwtToken.Replace("Bearer", string.Empty).Trim();
                //decode
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    decodedToken = handler.ReadToken(jwtToken) as JwtSecurityToken;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return decodedToken;
        }

        // Define const Key this should be private secret key stored in some safe place
        const string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";

        private static long GetTime(this DateTime dt)
        {
            long retval = 0;
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (dt.ToUniversalTime() - st);
            retval = (long)(t.TotalMilliseconds + 0.5);
            return retval / 1000;
        }

        public static string Generate(FormModel loginModel)
        {
            // Create Security key using private key above:
            // not that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //  Finally create a Token
            var header = new JwtHeader(credentials);

            /*{
  "kid": "tdiR8hcLuAHDwKC2AgZRC+wsa/JMGh/YFt13Te7TGhM=",
  "alg": "RS256"
}
            {
  "sub": "b8d25fa7-bbc9-47e5-9027-5e1b7b80ea10",
  "event_id": "25329fd4-db4a-4a0f-b160-b92a4def9aa2",
  "token_use": "access",
  "scope": "aws.cognito.signin.user.admin",
  "auth_time": 1571304016,
  "iss": "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_geMNBrF6G",
  "exp": 1571307616,
  "iat": 1571304016,
  "jti": "6a10d514-e143-448e-82fa-a87563756b68",
  "client_id": "i7jsqvgnglu2g1eovk1kie85e",
  "username": "SangLe"
} */

            //Some PayLoad that contain information about the customer
            var payload = new JwtPayload
            {
               { "sub ", 1 },   // user id
               { "event_id ", Guid.NewGuid().ToString() },
               // { "iss ", username },
               { "auth_time ", GetTime(DateTime.Now).ToString() },
               { "iat ", GetTime(DateTime.Now).ToString() },
               { "exp ", GetTime(DateTime.Now.AddMinutes(5d)).ToString()},
               { "iss ", "hello" },
               { "username ", loginModel.Username },
               // { "scope", "http://dummy.com/"},
            };

            //
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);

            return tokenString;
        }

    }
}

