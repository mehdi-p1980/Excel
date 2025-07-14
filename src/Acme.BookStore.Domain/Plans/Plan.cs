using System;
using Volo.Abp.Domain.Entities;

namespace Acme.BookStore.Plans
{
    public class Plan : Entity<Guid>
    {
        public string Name { get; set; }
        public PlanType Type { get; set; }
        public int? DurationDays { get; set; }
        public int? DocumentCount { get; set; }
        public decimal Price { get; set; }
    }
}
