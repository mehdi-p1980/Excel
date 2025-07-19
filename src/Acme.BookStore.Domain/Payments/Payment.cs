using System;
using Volo.Abp.Domain.Entities;

namespace Acme.BookStore.Payments
{
    public class Payment : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
    }
}