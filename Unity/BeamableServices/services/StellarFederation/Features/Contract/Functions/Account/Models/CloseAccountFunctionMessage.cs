using MongoDB.Bson;

namespace Beamable.StellarFederation.Features.Contract.Functions.Account.Models;

public record CloseAccountFunctionMessage(ObjectId TransactionId, Accounts.Models.Account Account) : IFunctionNativeMessage
{
    public string FunctionName => "close_account";
}