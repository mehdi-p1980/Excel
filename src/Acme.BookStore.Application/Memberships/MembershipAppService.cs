using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Acme.BookStore.Memberships
{
using Acme.BookStore.Accounting;
using Acme.BookStore.Plans;
using Acme.BookStore.Users;
using Volo.Abp.Identity;

public class MembershipAppService : CrudAppService<Membership, MembershipDto, Guid>, IMembershipAppService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IPlanAppService _planAppService;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IAccountingAppService _accountingAppService;
        private readonly IRepository<Account, Guid> _accountRepository;

        public MembershipAppService(
            IRepository<Membership, Guid> repository,
            ICurrentUser currentUser,
            IPlanAppService planAppService,
            IRepository<AppUser, Guid> userRepository,
            IdentityUserManager userManager,
            IAccountingAppService accountingAppService,
            IRepository<Account, Guid> accountRepository) : base(repository)
        {
            _currentUser = currentUser;
            _planAppService = planAppService;
            _userRepository = userRepository;
            _userManager = userManager;
            _accountingAppService = accountingAppService;
            _accountRepository = accountRepository;
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

                var plan = await _planAppService.GetAsync(membership.PlanId);
                var salesRevenueAccount = await _accountRepository.FirstOrDefaultAsync(a => a.Name == "Sales Revenue");
                var cashAccount = await _accountRepository.FirstOrDefaultAsync(a => a.Name == "Cash");

                if (salesRevenueAccount != null && cashAccount != null)
                {
                    var lines = new[]
                    {
                        new JournalEntryLineDto { AccountId = salesRevenueAccount.Id, Debit = plan.Price, Credit = 0 },
                        new JournalEntryLineDto { AccountId = cashAccount.Id, Debit = 0, Credit = plan.Price }
                    };
                    await _accountingAppService.CreateJournalEntryAsync(DateTime.UtcNow, $"Cancellation of plan {plan.Name}", lines);
                }
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
