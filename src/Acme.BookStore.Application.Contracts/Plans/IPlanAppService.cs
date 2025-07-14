using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Plans
{
    public interface IPlanAppService : ICrudAppService<PlanDto, Guid>
    {
        Task<PlanDto> GetTrialPlanAsync();
    }
}
