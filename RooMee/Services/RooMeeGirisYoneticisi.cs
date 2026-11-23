using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RooMee.Models;

namespace RooMee.Services;

// Askıya alınmış hesapların giriş yapmasını engeller.
// (Açık oturumlar, askıya alınırken güvenlik damgası yenilendiği için en geç 1 dakikada kapanır.)
public class RooMeeGirisYoneticisi : SignInManager<ApplicationUser>
{
    public RooMeeGirisYoneticisi(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<ApplicationUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public override async Task<bool> CanSignInAsync(ApplicationUser user)
    {
        if (user.AskiyaAlindi) return false;
        return await base.CanSignInAsync(user);
    }
}
