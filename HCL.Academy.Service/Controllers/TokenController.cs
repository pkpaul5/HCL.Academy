using System;
using System.Web.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HCL.Academy.DAL;
using HCL.Academy.Model;
namespace HCL.Academy.Service.Controllers
{
    /// <summary>
    /// This controller generates JWT token
    /// </summary>
    public class TokenController : ApiController
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="emailid"></param>
        /// <returns></returns>
        [HttpGet]
        public Object Get(string emailid)
        {
            SqlSvrDAL dal = new SqlSvrDAL();
            int id= dal.GetUserId(emailid);
            if (id > 0)
            {
               UserManager u= dal.GetUsersByID(id);
                //Create a List of Claims, Keep claims name short    
                //Set issued at date
                DateTime issuedAt = DateTime.UtcNow;
                //set the time when it expires
                DateTime expires = DateTime.UtcNow.AddDays(1);             
                var tokenHandler = new JwtSecurityTokenHandler();
                //create a identity and add claims to the user which we want to log in
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name,u.UserName)
            });

                const string sec = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
                var now = DateTime.UtcNow;
                var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
                var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

                //create the jwt
                var token =
                    (JwtSecurityToken)
                        tokenHandler.CreateJwtSecurityToken(issuer: "academy", audience: "academy",
                            subject: claimsIdentity, notBefore: issuedAt, expires: expires, signingCredentials: signingCredentials);
                var tokenString = tokenHandler.WriteToken(token);
                return tokenString;
             
            }
            else return null;

        }
    }
}
