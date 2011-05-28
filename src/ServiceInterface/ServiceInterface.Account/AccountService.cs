using Nuxleus.Data;
using Nuxleus.ServiceModel.Operations;
using Nuxleus.ServiceModel.Types;
using ServiceStack.ServiceInterface;
using Nuxleus.ServiceModel.Types.Account;

namespace Nuxleus.ServiceInterface
{
    public class AccountService
        : RestServiceBase<Account>
    {
        public IDataRepository Repository { get; set; }

        public override object OnGet (Account request)
        {
            return new AccountResponse
            {
                AccountInfo = Repository.GetOrCreateAccount(request),
            };
        }

        public override object OnPost (Account request)
        {
            return new AccountResponse
            {
                AccountInfo = Repository.GetOrCreateAccount(request), 
            };
        }

        public override object OnPut (Account request)
        {
            return new AccountResponse
            {
                AccountInfo = Repository.GetOrCreateAccount(request),
            };
        }
    }
}