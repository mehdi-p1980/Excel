using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Plans
{
    public class PlanAppService : CrudAppService<Plan, PlanDto, Guid>, IPlanAppService
    {
        public PlanAppService(IRepository<Plan, Guid> repository) : base(repository)
        {
        }

        public async Task<PlanDto> GetTrialPlanAsync()
        {
            var plan = await Repository.FirstOrDefaultAsync(p => p.Name == "Trial");
            return ObjectMapper.Map<Plan, PlanDto>(plan);
        }
    }
}
