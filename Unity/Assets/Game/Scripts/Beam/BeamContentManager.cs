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

        public List<PlantInfo> CropNfContent { get; private set; }
        
        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            await SyncContentAsync(ct);
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
                var cropDataPath = cropResolved.CustomProperties.GetValueOrDefault("DataSource", "");
                var data = Resources.Load<CropsData>(cropDataPath);
                CropNfContent.Add(new PlantInfo
                {
                    contentId = cropResolved.Id,
                    sellingPrice = cropResolved.CustomProperties.TryGetValue("SellingPrice", out var sellingPrice) ? int.Parse(sellingPrice) : 0,
                    cropData = data,
                    seedsToPlant = data.startingSeedsAmount,
                });
            }
        }
        
        public PlantInfo GetCropInfo(string contentId)
        {
            return CropNfContent.Find(c => c.contentId == contentId);
        }
    }
}