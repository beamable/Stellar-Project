using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Server.Content;
using Beamable.StellarFederation.Caching;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Contract.Models;
using StellarFederationCommon;
// ReSharper disable MemberCanBePrivate.Global

namespace Beamable.StellarFederation.Features.Content;

public class BeamContentService : IService
{
    private readonly ContentService _contentService;

    public const string FederationContentLocal = "stellar.federation.content.local";

    public BeamContentService(ContentService contentService)
    {
        _contentService = contentService;
    }

    public async Task<IEnumerable<ContentContractsModel>> FetchFederationContentForContracts()
    {
        var federationContent = await FetchFederationContentLocal();
        var currencies = federationContent.Where(item => item is CurrencyContent)
            .Select(item => new ContentContractsModel(item.Id, item));

        var itemsByType = federationContent
            .Where(item => item is ItemContent)
            .GroupBy(item =>
            {
                var idParts = item.Id.Split('.');
                return string.Join(".", idParts.Take(idParts.Length - 1));
            })
            .Select(g => new ContentContractsModel(g.Key, g.First()));

        return currencies.Concat(itemsByType);
    }

    public async Task<IEnumerable<IContentObject>> FetchFederationContentForState(MicroserviceInfo microserviceInfo)
    {
        var federationContent = await FetchFederationContentLocal();
        var currencies = federationContent
            .Where(item => item is CurrencyContent coin && coin.federation.HasValue &&
                           coin.federation.Value.Service == microserviceInfo.MicroserviceName &&
                           coin.federation.Value.Namespace == microserviceInfo.MicroserviceNamespace);

        var itemsByType = federationContent
            .Where(item => item is ItemContent)
            .GroupBy(item => {
                var idParts = item.Id.Split('.');
                return string.Join(".", idParts.Take(idParts.Length - 1));
            })
            .ToDictionary(g => g.Key, g => g.ToList());

        var matchingItems = itemsByType
            .Select(kvp => kvp.Value.FirstOrDefault(co => co is ItemContent item && item.federation.HasValue &&
                                                          item.federation.Value.Service == microserviceInfo.MicroserviceName &&
                                                          item.federation.Value.Namespace == microserviceInfo.MicroserviceNamespace))
            .Where(co => co is not null)
            .Cast<IContentObject>();
        return matchingItems.Concat(currencies);
    }

    private async Task<List<IContentObject>> FetchFederationContentLocal()
    {
        return await GlobalCache.GetOrCreate<List<IContentObject>>(FederationContentLocal, async _ =>
        {
            var federatedTypes = StellarFederationCommonHelper.GetFederationTypes();
            var manifest = await _contentService.GetManifest(new ContentQuery
            {
                TypeConstraints = federatedTypes
            });

            var result = new List<IContentObject>();
            foreach (var clientContentInfo in manifest.entries.Where(
                         item => item.contentId.StartsWith("currency") ||
                                 item.contentId.StartsWith("items")))
            {
                var contentRef = await clientContentInfo.Resolve();
                result.Add(contentRef);
            }
            return result;
        }, TimeSpan.FromDays(1)) ?? [];
    }

    public async Task<List<IContentObject>> GetContentObjects(IEnumerable<string> contentId)
    {
        var allContent = await FetchFederationContentLocal();
        var content= allContent.Where( c => contentId.Contains(c.Id));
        return content.ToList();
    }
}