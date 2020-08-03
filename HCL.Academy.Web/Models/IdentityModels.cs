using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using System.Configuration;
using System;
namespace HCL.Academy.Model
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
  
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(GetDBConnectionString(), throwIfV1Schema: false)
        {

        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public static string GetDBConnectionString()
        {
            string strConnectionString = string.Empty;
            string keyvaultuse = ConfigurationManager.AppSettings["DBCONSTRFROMAZKEYVAULT"].ToString();
            if (keyvaultuse.ToUpper() == "TRUE")
            {
                string BASESECRETURI = ConfigurationManager.AppSettings["KeyVaultURL"].ToString();

                SecretClientOptions options = new SecretClientOptions()
                {
                    Retry =
                        {
                            Delay= TimeSpan.FromSeconds(2),
                            MaxDelay = TimeSpan.FromSeconds(16),
                            MaxRetries = 5,
                            Mode = RetryMode.Exponential
                         }
                };
                var client = new SecretClient(new Uri(BASESECRETURI), new DefaultAzureCredential(), options);
                KeyVaultSecret secret = client.GetSecret("academydbconstr");
                strConnectionString = secret.Value;
            }
            else
                strConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            return strConnectionString;
        }
    }
}