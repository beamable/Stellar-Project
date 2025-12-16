// using System.Threading.Tasks;
// using Beamable.Common;
// using Beamable.StellarFederation.Features.Accounts.Storage;
// using Beamable.StellarFederation.Features.Stellar;
//
// namespace Beamable.StellarFederation.Features.Accounts;
//
// //Probably not needed because there is no mempool, sequence numbers will update on GetStellarAccount call
// public class SequenceService : IService
// {
//     private readonly SequenceCollection _sequenceCollection;
//     private readonly StellarService _stellarService;
//
//     public SequenceService(SequenceCollection sequenceCollection, StellarService stellarService)
//     {
//         _sequenceCollection = sequenceCollection;
//         _stellarService = stellarService;
//     }
//
//     public async Task<long> GetNextNonceAsync(string address)
//     {
//         var nextValue = await _sequenceCollection.GetNextIfNoErrors(address);
//         if (nextValue is not null)
//             return nextValue.Value;
//
//         var errorSequence = await _sequenceCollection.PopError(address);
//         BeamableLogger.LogWarning("Sequence has errors. Trying to reuse the error sequence value {n}.", errorSequence);
//         if (errorSequence is not null)
//             return errorSequence.Value;
//
//         BeamableLogger.LogWarning("Unable to fetch error sequence. Fetching sequence again.");
//         return await GetNextNonceAsync(address);
//     }
//
//     public async Task Set(string address, long sequence = 0)
//     {
//         if (sequence > 0)
//         {
//             await _sequenceCollection.Set(address, sequence);
//             return;
//         }
//
//         var stellarAccount = await _stellarService.GetStellarAccount(address);
//         await _sequenceCollection.Set(address, stellarAccount.SequenceNumber - 1);
//     }
//
//     public async Task PushErrorAsync(long error, string address)
//     {
//         await _sequenceCollection.PushError(address, error);
//     }
// }