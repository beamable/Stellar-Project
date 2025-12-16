using System;

namespace StellarFederationCommon.Store
{
    [Serializable]
    public class GetListingsResponse
    {
        public StoreView[] stores;
    }

    [Serializable]
    public class StoreView
    {
        public string title;
        public string symbol;
        public long nextDeltaSeconds;
        public long secondsRemain;
        public ListingView[] listings;
    }

    [Serializable]
    public class ListingView
    {
        public string symbol;
        public bool active;
        public long secondsActive;
        public long secondsRemain;
        public int purchasesRemain;
        public int cooldown;
        public ClientDataView[] clientData;
        public OfferView offer;
    }

    [Serializable]
    public class OfferView
    {
        public string symbol;
        public string[] titles;
        public string[] descriptions;
        public OfferPriceView price;
        public ObtainCurrencyView[] obtainCurrency;
        public ObtainItemsView[] obtainItems;
    }

    [Serializable]
    public class ObtainItemsView
    {
        public string contentId;
        public ItemPropertyView[] properties;
    }

    [Serializable]
    public class ObtainCurrencyView
    {
        public string symbol;
        public long amount;
    }

    [Serializable]
    public class OfferPriceView
    {
        public string symbol;
        public string type;
        public int amount;
        public int[] schedule;
    }

    [Serializable]
    public class ClientDataView
    {
        public string name;
        public string value;
    }

    [Serializable]
    public class ItemPropertyView
    {
        public string name;
        public string value;
    }
}