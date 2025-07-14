using System;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Users;

namespace Acme.BookStore.Memberships
{
    public class MembershipExpirationNotifier_Tests : BookStoreApplicationTestBase
    {
        private readonly MembershipExpirationNotifier _membershipExpirationNotifier;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly IRepository<Membership, Guid> _membershipRepository;

        public MembershipExpirationNotifier_Tests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _currentUserMock = new Mock<ICurrentUser>();
            _membershipRepository = GetRequiredService<IRepository<Membership, Guid>>();

            _membershipExpirationNotifier = new MembershipExpirationNotifier(
                _membershipRepository,
                _emailSenderMock.Object,
                _currentUserMock.Object
            );
        }

        [Fact]
        public async Task Should_Send_Email_To_Users_With_Expiring_Memberships()
        {
            // Arrange
            var expiringMembership = new Membership
            {
                IsActive = true,
                EndDate = DateTime.UtcNow.AddDays(3),
                UserId = Guid.NewGuid()
            };
            await _membershipRepository.InsertAsync(expiringMembership);

            // Act
            await _membershipExpirationNotifier.NotifyUsers();

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    "test@example.com",
                    "Your membership is expiring soon",
                    "Your membership will expire in 3 days. Please renew to continue enjoying our services."
                ),
                Times.Once
            );
        }
    }
}
