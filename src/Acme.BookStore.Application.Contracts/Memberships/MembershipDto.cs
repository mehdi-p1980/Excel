using System;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Memberships
{
    public class MembershipDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? RemainingDocuments { get; set; }
        public bool IsActive { get; set; }
    }
}
