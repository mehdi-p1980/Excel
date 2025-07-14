using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Identity;

namespace Acme.BookStore.Account
{
using Acme.BookStore.Memberships;

public class CustomAccountAppService : AccountAppService
    {
        private readonly IMembershipAppService _membershipAppService;

        public CustomAccountAppService(IdentityUserManager userManager, IIdentityRoleRepository roleRepository, IAccountSmsService accountSmsService, IdentitySecurityLogManager identitySecurityLogManager, IMembershipAppService membershipAppService) : base(userManager, roleRepository, accountSmsService, identitySecurityLogManager)
        {
            _membershipAppService = membershipAppService;
        }

        public override async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
        {
            var user = await base.RegisterAsync(input);

            var trialPlan = await _planAppService.GetTrialPlanAsync();
            if (trialPlan != null)
            {
                await _membershipAppService.ExtendMembership(trialPlan.Id);
            }

            return user;
        }
    }
}
