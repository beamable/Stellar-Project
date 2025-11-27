using MongoDB.Bson;

namespace Beamable.StellarFederation.Features.Contract.Functions.Account.Models;

public record CreateAccountFunctionMessage(ObjectId TransactionId, Accounts.Models.Account NewAccount) : IFunctionNativeMessage
{
    public string FunctionName => StaticFunctionName;
    public const string StaticFunctionName = "create_account";
}