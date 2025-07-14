using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Acme.BookStore.Memberships
{
    public class MembershipAppService_Tests : BookStoreApplicationTestBase
    {
        private readonly IMembershipAppService _membershipAppService;

        public MembershipAppService_Tests()
        {
            _membershipAppService = GetRequiredService<IMembershipAppService>();
        }

        [Fact]
        public async Task Should_Get_List_Of_Memberships()
        {
            // Act
            var result = await _membershipAppService.GetListAsync(new ());

            // Assert
            result.TotalCount.ShouldBeGreaterThan(0);
            result.Items.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Check_Membership_Status()
        {
            // Act
            var result = await _membershipAppService.CheckMembershipStatus();

            // Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task Should_Extend_Membership()
        {
            // Arrange
            var plan = new Plans.PlanDto { Id = Guid.NewGuid(), Type = Plans.PlanType.Daily, DurationDays = 30 };

            // Act
            await _membershipAppService.ExtendMembership(plan.Id);

            // Assert
            var membership = await _membershipAppService.GetActiveMembership();
            membership.ShouldNotBeNull();
            membership.EndDate.ShouldBeGreaterThan(DateTime.UtcNow);
        }

        [Fact]
        public async Task Should_Cancel_Membership()
        {
            // Act
            await _membershipAppService.CancelMembership();

            // Assert
            var membership = await _membershipAppService.GetActiveMembership();
            membership.ShouldBeNull();
        }
    }
}
