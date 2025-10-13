using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Server.Content;
using Beamable.StellarFederation.Caching;
using Beamable.StellarFederation.Features.Contract.Models;
using StellarFederationCommon;

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
            .Select(item => new ContentContractsModel(item));

        var itemsByType = federationContent
            .Where(item => item is ItemContent)
            .GroupBy(item =>
            {
                var idParts = item.Id.Split('.');
                return string.Join(".", idParts.Take(idParts.Length - 1));
            })
            .Select(g => new ContentContractsModel(g.First()));

        return currencies.Concat(itemsByType);
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
}