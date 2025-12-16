using Beamable.StellarFederation.Features.Contract.Functions;
using MongoDB.Bson;

namespace Beamable.StellarFederation.Features.Contract.Models;

public record TransferNativeFunctionMessage(ObjectId TransactionId, string To, long Amount) : IFunctionNativeMessage
{
    public string FunctionName => "transfer_native";
}