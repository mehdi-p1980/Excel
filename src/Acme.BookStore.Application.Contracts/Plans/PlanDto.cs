using System;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Plans
{
    public class PlanDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public PlanType Type { get; set; }
        public int? DurationDays { get; set; }
        public int? DocumentCount { get; set; }
        public decimal Price { get; set; }
    }
}
