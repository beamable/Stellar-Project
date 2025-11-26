using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions;

public interface IFunctionMessage
{
    ObjectId TransactionId { get; }
    string ContentId { get; }
    string FunctionName { get; }
    SCVal[] ToArgs();
}

public interface IFunctionNativeMessage
{
    ObjectId TransactionId { get; }
    string FunctionName { get; }
}