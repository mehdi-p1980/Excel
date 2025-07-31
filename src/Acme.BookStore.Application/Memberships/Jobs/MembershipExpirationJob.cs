using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.BookStore.Plans;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Memberships.Jobs
{
    public class MembershipExpirationJob : IBackgroundJob, ITransientDependency
    {
        private readonly IRepository<Membership, Guid> _membershipRepository;
        private readonly IRepository<Plan, Guid> _planRepository;

        public MembershipExpirationJob(
            IRepository<Membership, Guid> membershipRepository,
            IRepository<Plan, Guid> planRepository)
        {
            _membershipRepository = membershipRepository;
            _planRepository = planRepository;
        }

        public async Task ExecuteAsync()
        {
            var memberships = await _membershipRepository.GetListAsync(m => m.IsActive);
            foreach (var membership in memberships)
            {
                var plan = await _planRepository.GetAsync(membership.PlanId);
                if (plan == null)
                {
                    continue;
                }

                if (plan.Type == PlanType.Daily)
                {
                    if (membership.EndDate < DateTime.UtcNow)
                    {
                        membership.IsActive = false;
                        await _membershipRepository.UpdateAsync(membership);
                    }
                }
                else
                {
                    if (membership.RemainingDocuments <= 0)
                    {
                        membership.IsActive = false;
                        await _membershipRepository.UpdateAsync(membership);
                    }
                }
            }
        }
    }
}
