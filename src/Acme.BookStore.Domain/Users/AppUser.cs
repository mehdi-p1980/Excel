using Volo.Abp.Identity;

namespace Acme.BookStore.Users
{
    public class AppUser : IdentityUser
    {
        public bool HasUsedTrialPlan { get; set; }
    }
}
