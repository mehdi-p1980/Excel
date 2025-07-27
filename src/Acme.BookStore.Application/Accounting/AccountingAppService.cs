using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Accounting
{
    public class AccountingAppService : ApplicationService, IAccountingAppService
    {
        private readonly IRepository<Account, Guid> _accountRepository;
        private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;

        public AccountingAppService(
            IRepository<Account, Guid> accountRepository,
            IRepository<JournalEntry, Guid> journalEntryRepository)
        {
            _accountRepository = accountRepository;
            _journalEntryRepository = journalEntryRepository;
        }

        public async Task CreateAccountAsync(string name, AccountType type)
        {
            var account = new Account
            {
                Name = name,
                Type = type
            };

            await _accountRepository.InsertAsync(account);
        }

        public async Task CreateJournalEntryAsync(DateTime date, string description, JournalEntryLineDto[] lines)
        {
            var journalEntryLines = new List<JournalEntryLine>();
            foreach (var line in lines)
            {
                journalEntryLines.Add(new JournalEntryLine
                {
                    AccountId = line.AccountId,
                    Debit = line.Debit,
                    Credit = line.Credit
                });
            }

            var journalEntry = new JournalEntry
            {
                Date = date,
                Description = description,
                Lines = journalEntryLines
            };

            await _journalEntryRepository.InsertAsync(journalEntry);
        }
    }
}
