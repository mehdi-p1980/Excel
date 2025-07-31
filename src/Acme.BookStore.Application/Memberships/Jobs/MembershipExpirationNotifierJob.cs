using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.BookStore.Plans;
using Microsoft.Extensions.Configuration;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Users;

namespace Acme.BookStore.Memberships.Jobs
{
    public class MembershipExpirationNotifierJob : IBackgroundJob, ITransientDependency
    {
        private readonly IRepository<Membership, Guid> _membershipRepository;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<IUser, Guid> _userRepository;
        private readonly IRepository<Plan, Guid> _planRepository;
        private readonly IConfiguration _configuration;

        public MembershipExpirationNotifierJob(
            IRepository<Membership, Guid> membershipRepository,
            IEmailSender emailSender,
            IRepository<IUser, Guid> userRepository,
            IRepository<Plan, Guid> planRepository,
            IConfiguration configuration)
        {
            _membershipRepository = membershipRepository;
            _emailSender = emailSender;
            _userRepository = userRepository;
            _planRepository = planRepository;
            _configuration = configuration;
        }

        public async Task ExecuteAsync()
        {
            var memberships = await _membershipRepository.GetListAsync(m => m.IsActive);
            var notificationThreshold = _configuration.GetValue<int>("App:NotificationThreshold");

            foreach (var membership in memberships)
            {
                var plan = await _planRepository.GetAsync(membership.PlanId);
                if (plan == null)
                {
                    continue;
                }

                var user = await _userRepository.GetAsync(membership.UserId);
                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    continue;
                }

                if (plan.Type == PlanType.Daily)
                {
                    if (membership.EndDate.HasValue && membership.EndDate.Value.Date == DateTime.UtcNow.Date.AddDays(3))
                    {
                        await _emailSender.SendAsync(
                            user.Email,
                            "Membership Expiration Notification",
                            "Your membership is about to expire in 3 days."
                        );
                    }
                }
                else
                {
                    if (membership.RemainingDocuments <= notificationThreshold)
                    {
                        await _emailSender.SendAsync(
                            user.Email,
                            "Membership Expiration Notification",
                            $"You have {membership.RemainingDocuments} documents remaining in your plan."
                        );
                    }
                }
            }
        }
    }
}
