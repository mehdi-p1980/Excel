using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Acme.BookStore.Accounting
{
    public class JournalEntry : Entity<Guid>
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public ICollection<JournalEntryLine> Lines { get; set; }
    }

    public class JournalEntryLine : Entity<Guid>
    {
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
