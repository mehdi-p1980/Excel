using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Acme.BookStore.Plans
{
    public class PlanAppService_Tests : BookStoreApplicationTestBase
    {
        private readonly IPlanAppService _planAppService;

        public PlanAppService_Tests()
        {
            _planAppService = GetRequiredService<IPlanAppService>();
        }

        [Fact]
        public async Task Should_Get_List_Of_Plans()
        {
            // Act
            var result = await _planAppService.GetListAsync(new ());

            // Assert
            result.TotalCount.ShouldBeGreaterThan(0);
            result.Items.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Get_Trial_Plan()
        {
            // Act
            var result = await _planAppService.GetTrialPlanAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Trial");
        }
    }
}
