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
    public class MembershipExpirationNotifierJob : IBackgroundJob, ITransientDependency
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

        public async Task ExecuteAsync()
        {
            var memberships = await _membershipRepository.GetListAsync(m => m.IsActive && m.EndDate.HasValue && m.EndDate.Value.Date == DateTime.UtcNow.Date.AddDays(3));
            foreach (var membership in memberships)
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
