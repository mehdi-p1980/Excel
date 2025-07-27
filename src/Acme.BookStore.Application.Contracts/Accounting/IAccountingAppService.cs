using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Accounting
{
    public interface IAccountingAppService : IApplicationService
    {
        Task CreateAccountAsync(string code, string name, AccountType type);
        Task CreateJournalEntryAsync(DateTime date, string description, JournalEntryLineDto[] lines);
    }

    public class JournalEntryLineDto
    {
        public Guid AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
