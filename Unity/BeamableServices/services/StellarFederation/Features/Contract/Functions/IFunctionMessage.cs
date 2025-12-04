using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions;

public interface IFunctionMessage
{
    ObjectId[] TransactionIds { get; }
    string ContentId { get; }
    string FunctionName { get; }
    string ConcurrencyKey { get; }
    SCVal[] ToArgs();
}

public interface IFunctionNativeMessage
{
    ObjectId TransactionId { get; }
    string FunctionName { get; }
}

public interface IFunctionViewMessage
{
    string FunctionName { get; }
    string ContentId { get; }
    SCVal[] ToArgs();
}