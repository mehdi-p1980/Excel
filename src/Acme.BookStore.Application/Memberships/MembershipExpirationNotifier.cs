using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Users;

namespace Acme.BookStore.Memberships
{
    public class MembershipExpirationNotifier
    {
        private readonly IRepository<Membership, Guid> _membershipRepository;
        private readonly IEmailSender _emailSender;
        private readonly ICurrentUser _currentUser;

        public MembershipExpirationNotifier(
            IRepository<Membership, Guid> membershipRepository,
            IEmailSender emailSender,
            ICurrentUser currentUser)
        {
            _membershipRepository = membershipRepository;
            _emailSender = emailSender;
            _currentUser = currentUser;
        }

        public async Task NotifyUsers()
        {
            var expiringMemberships = await _membershipRepository.GetListAsync(m =>
                m.IsActive &&
                m.EndDate.HasValue &&
                m.EndDate.Value.Date == DateTime.UtcNow.Date.AddDays(3));

            foreach (var membership in expiringMemberships)
            {
                // TODO: Get the user's email address
                var userEmail = "test@example.com";

                await _emailSender.SendAsync(
                    userEmail,
                    "Your membership is expiring soon",
                    "Your membership will expire in 3 days. Please renew to continue enjoying our services."
                );
            }
        }
    }
}
