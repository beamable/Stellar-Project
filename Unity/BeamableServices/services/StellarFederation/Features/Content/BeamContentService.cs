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
            .Select(g => new
            {
                Content = new ContentContractsModel(g.Key, g.First())
            });

        return currencies; // ADD itemsByType when contract is created
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