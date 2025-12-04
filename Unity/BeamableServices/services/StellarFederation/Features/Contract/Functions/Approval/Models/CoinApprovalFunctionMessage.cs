using System.Linq;
using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions.Approval.Models;

// public record CoinApprovalFunctionMessage(ObjectId TransactionId, string ContentId, string[] From, string[] Spender, long[] Amount, uint[] ExpirationLedger) : IFunctionMessage
// {
//     public string FunctionName => "batch_approve";
//     public SCVal[] ToArgs() =>
//     [
//         new SCVec(
//             From.Zip(Spender, (from, spender) => (from, spender))
//                 .Zip(Amount, (pair, amount) => (pair.from, pair.spender, amount))
//                 .Zip(ExpirationLedger, (triple, expiration) =>
//                     new SCVec([
//                         new ScAccountId(triple.from),
//                         new ScAccountId(triple.spender),
//                         new SCInt128(triple.amount.ToString()),
//                         new SCUint32(expiration)
//                     ])
//                 ).ToArray<SCVal>()
//         )
//     ];
// }