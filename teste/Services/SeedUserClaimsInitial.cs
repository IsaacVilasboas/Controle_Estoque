using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace teste.Services
{
    public class SeedUserClaimsInitial : ISeedUserClaimsInitial
    {
        private readonly UserManager<IdentityUser> userManager;

        public SeedUserClaimsInitial(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task SeedUserClaims()
        {
            try
            {
                IdentityUser user1 = await userManager.FindByNameAsync("admin");
                if(user1 is not null)
                {
                    var claimList = (await userManager.GetClaimsAsync(user1)).Select(x => x.Type);

                    if (!claimList.Contains("CadastradoEm"))
                    {
                        var claimResult = await userManager.AddClaimAsync(user1, new Claim("CadastradoEm", "09/15/2014"));
                    }
                    if (!claimList.Contains("IsAdmin"))
                    {
                        var claimResult2 = await userManager.AddClaimAsync(user1, new Claim("IsAdmin", "true"));
                    }
                }

                IdentityUser user2 = await userManager.FindByNameAsync("admin");
                if (user2 is not null)
                {
                    var claimList = (await userManager.GetClaimsAsync(user2)).Select(x => x.Type);

                    if (!claimList.Contains("IsAdmin"))
                    {
                        var claimResult = await userManager.AddClaimAsync(user2, new Claim("IsAdmin", "false"));
                    }
                    if (!claimList.Contains("IsFuncionario"))
                    {
                        var claimResult = await userManager.AddClaimAsync(user2, new Claim("IsFuncionario", "true"));
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
