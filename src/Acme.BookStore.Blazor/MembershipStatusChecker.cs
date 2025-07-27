using System.Threading.Tasks;
using Acme.BookStore.Memberships;
using Microsoft.AspNetCore.Components;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Acme.BookStore.Blazor
{
    public class MembershipStatusChecker : ITransientDependency
    {
        private readonly IMembershipAppService _membershipAppService;
        private readonly NavigationManager _navigationManager;
        private readonly ICurrentUser _currentUser;

        public MembershipStatusChecker(
            IMembershipAppService membershipAppService,
            NavigationManager navigationManager,
            ICurrentUser currentUser)
        {
            _membershipAppService = membershipAppService;
            _navigationManager = navigationManager;
            _currentUser = currentUser;
        }

        public async Task CheckAndRedirect()
        {
            if (_currentUser.IsAuthenticated)
            {
                var hasActiveMembership = await _membershipAppService.CheckMembershipStatus();
                if (!hasActiveMembership)
                {
                    _navigationManager.NavigateTo("/plan-selection", forceLoad: true);
                }
            }
        }
    }
}
