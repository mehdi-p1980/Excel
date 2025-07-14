using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Memberships
{
    public interface IMembershipAppService : ICrudAppService<MembershipDto, Guid>
    {
        Task<bool> CheckMembershipStatus();
        Task ExtendMembership(Guid planId);
        Task CancelMembership();
        Task<MembershipDto> GetActiveMembership();
    }
}
