using DonationManagerWebHooks.Models;
using System.Threading.Tasks;

namespace DonationManagerWebHooks.WordPressService
{
    public interface IDonationManagerService
    {
        Task<Supporter> SubscriptionAsync(Supporter supporter);
        Task<Appeal> AppealAsync(Appeal appeal);
    }
}