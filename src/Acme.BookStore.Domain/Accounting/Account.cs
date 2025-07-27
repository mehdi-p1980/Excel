using System;
using Volo.Abp.Domain.Entities;

namespace Acme.BookStore.Accounting
{
    public class Account : Entity<Guid>
    {
        public string Name { get; set; }
        public AccountType Type { get; set; }
        public decimal Balance { get; set; }
    }

    public enum AccountType
    {
        Asset,
        Liability,
        Equity,
        Revenue,
        Expense
    }
}
