using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Acme.BookStore.Plans;
using Acme.BookStore.Users;
using Volo.Abp;
using Volo.Abp.Identity;

namespace Acme.BookStore.Memberships
{

    public class MembershipAppService : CrudAppService<Membership, MembershipDto, Guid>, IMembershipAppService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IPlanAppService _planAppService;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IdentityUserManager _userManager;

        public MembershipAppService(
            IRepository<Membership, Guid> repository,
            ICurrentUser currentUser,
            IPlanAppService planAppService,
            IRepository<AppUser, Guid> userRepository,
            IdentityUserManager userManager) : base(repository)
        {
            _currentUser = currentUser;
            _planAppService = planAppService;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<bool> CheckMembershipStatus()
        {
            var userId = _currentUser.Id.GetValueOrDefault();
            var membership = await Repository.FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);
            return membership != null;
        }

        public async Task ExtendMembership(Guid planId)
        {
            var userId = _currentUser.Id.GetValueOrDefault();
            var user = await _userRepository.GetAsync(userId);
            var plan = await _planAppService.GetAsync(planId);

            if (plan.Name == "Trial" && user.HasUsedTrialPlan)
            {
                throw new UserFriendlyException("You have already used the trial plan.");
            }

            var membership = await Repository.FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);
            if (membership == null)
            {
                // Create a new membership
                membership = new Membership
                {
                    UserId = userId,
                    PlanId = planId,
                    StartDate = DateTime.UtcNow,
                    IsActive = true
                };

                if (plan.Type == Plans.PlanType.Daily)
                {
                    membership.EndDate = DateTime.UtcNow.AddDays(plan.DurationDays.Value);
                }
                else
                {
                    membership.RemainingDocuments = plan.DocumentCount.Value;
                }

                await Repository.InsertAsync(membership);
            }
            else
            {
                // Extend the existing membership
                if (plan.Type == PlanType.Daily)
                {
                    membership.EndDate = membership.EndDate.Value.AddDays(plan.DurationDays.Value);
                }
                else
                {
                    membership.RemainingDocuments += plan.DocumentCount.Value;
                }

                await Repository.UpdateAsync(membership);
            }

            if (plan.Name == "Trial")
            {
                user.HasUsedTrialPlan = true;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task CancelMembership()
        {
            var userId = _currentUser.Id.GetValueOrDefault();
            var membership = await Repository.FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);
            if (membership != null)
            {
                membership.IsActive = false;
                membership.EndDate = DateTime.UtcNow;
                await Repository.UpdateAsync(membership);
            }
        }

        public async Task<MembershipDto> GetActiveMembership()
        {
            var userId = _currentUser.Id.GetValueOrDefault();
            var membership = await Repository.FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);
            return ObjectMapper.Map<Membership, MembershipDto>(membership);
        }
    }
}
