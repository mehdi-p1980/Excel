using System;

using Volo.Abp.Domain.Entities;

namespace Acme.BookStore.Memberships
{
    public class Membership : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? RemainingDocuments { get; set; }
        public bool IsActive { get; set; }
    }
}
