using System.Collections.Generic;
using System.Threading;
using Beamable.Common.Content;
using Cysharp.Threading.Tasks;
using Farm.Managers;
using StellarFederationCommon.FederationContent;
using UnityEngine;

namespace Farm.Beam
{
    public class BeamContentManager : BeamManagerBase
    {
        [SerializeField] private CropItemRef[] cropsNfContentRefs;
        [SerializeField] private CropItemRef[] cropsContentRefs;

        private const string DataPathProp = "DataSource";
        private const string YieldSellingPriceProp = "SellingPrice";
        private const string SeedBuyingPriceProp = "SeedPrice";
        
        public List<PlantInfo> CropNfContent { get; private set; }
        
        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            await SyncContentAsync(ct);

            IsReady = true;
        }

        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
            CropNfContent = new List<PlantInfo>();
        }

        private async UniTask SyncContentAsync(CancellationToken ct)
        {
            CropNfContent = new List<PlantInfo>();

            foreach (var cropRef in cropsNfContentRefs)
            {
                var cropResolved = await cropRef.Resolve();
                var cropDataPath = cropResolved.CustomProperties.GetValueOrDefault(DataPathProp, "");
                var data = Resources.Load<CropsData>(cropDataPath);
                CropNfContent.Add(new PlantInfo
                {
                    contentId = cropResolved.Id,
                    yieldSellPrice = cropResolved.CustomProperties.TryGetValue(YieldSellingPriceProp, out var sellingPrice) ? int.Parse(sellingPrice) : 0,
                    seedBuyPrice = cropResolved.CustomProperties.TryGetValue(SeedBuyingPriceProp, out var seedBuyPrice) ? int.Parse(seedBuyPrice) : 0,
                    cropData = data,
                    seedsToPlant = data.startingSeedsAmount,
                    yieldAmount = 0
                });
            }
        }
        
        public PlantInfo GetCropInfo(string contentId)
        {
            return CropNfContent.Find(c => c.contentId == contentId);
        }
    }
}