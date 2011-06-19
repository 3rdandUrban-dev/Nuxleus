using Nuxleus.Data;
using Nuxleus.ServiceModel.Operations;
using Nuxleus.ServiceModel.Types;
using ServiceStack.ServiceInterface;
using Nuxleus.ServiceModel.Types.Account;

namespace Nuxleus.ServiceInterface
{
    public class ProfileNameStatusService : RestServiceBase<AccountProfileNameStatus>
    {
        public IDataRepository Repository { get; set; }

        public override object OnGet (AccountProfileNameStatus accountName)
        {
         
            return new AccountProfileNameStatusResponse
             
            {
                ProfileNameInfo = Repository.GetProfileNameAvailability(accountName),
             
            };
        }
    }
}