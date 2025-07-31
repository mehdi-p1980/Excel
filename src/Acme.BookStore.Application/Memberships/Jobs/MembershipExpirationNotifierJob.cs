using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Users;

namespace Acme.BookStore.Memberships.Jobs
{
    public class MembershipExpirationNotifierJob : IBackgroundJob<Guid>, ITransientDependency
    {
        private readonly IRepository<Membership, Guid> _membershipRepository;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<IUser, Guid> _userRepository;

        public MembershipExpirationNotifierJob(
            IRepository<Membership, Guid> membershipRepository,
            IEmailSender emailSender,
            IRepository<IUser, Guid> userRepository)
        {
            _membershipRepository = membershipRepository;
            _emailSender = emailSender;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(Guid membershipId)
        {
            var membership = await _membershipRepository.GetAsync(membershipId);
            if (membership == null || !membership.IsActive)
            {
                return;
            }

            if (membership.EndDate.HasValue && membership.EndDate.Value.Date == DateTime.UtcNow.Date.AddDays(3))
            {
                var user = await _userRepository.GetAsync(membership.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    await _emailSender.SendAsync(
                        user.Email,
                        "Membership Expiration Notification",
                        "Your membership is about to expire in 3 days."
                    );
                }
            }
        }
    }
}
