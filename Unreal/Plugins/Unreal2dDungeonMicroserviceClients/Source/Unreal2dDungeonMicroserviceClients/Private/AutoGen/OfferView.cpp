
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/OfferView.h"
#include "Serialization/BeamJsonUtils.h"




void UOfferView::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("symbol"), Symbol, Serializer);
	UBeamJsonUtils::SerializeUObject<UOfferPriceView*>("price", Price, Serializer);
	UBeamJsonUtils::SerializeArray<FString>(TEXT("titles"), Titles, Serializer);
	UBeamJsonUtils::SerializeArray<FString>(TEXT("descriptions"), Descriptions, Serializer);
	UBeamJsonUtils::SerializeArray<UObtainCurrencyView*>(TEXT("obtainCurrency"), ObtainCurrency, Serializer);
	UBeamJsonUtils::SerializeArray<UObtainItemsView*>(TEXT("obtainItems"), ObtainItems, Serializer);
}

void UOfferView::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("symbol"), Symbol, Serializer);
	UBeamJsonUtils::SerializeUObject<UOfferPriceView*>("price", Price, Serializer);
	UBeamJsonUtils::SerializeArray<FString>(TEXT("titles"), Titles, Serializer);
	UBeamJsonUtils::SerializeArray<FString>(TEXT("descriptions"), Descriptions, Serializer);
	UBeamJsonUtils::SerializeArray<UObtainCurrencyView*>(TEXT("obtainCurrency"), ObtainCurrency, Serializer);
	UBeamJsonUtils::SerializeArray<UObtainItemsView*>(TEXT("obtainItems"), ObtainItems, Serializer);		
}

void UOfferView::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("symbol"), Bag, Symbol);
	UBeamJsonUtils::DeserializeUObject<UOfferPriceView*>("price", Bag, Price, OuterOwner);
	UBeamJsonUtils::DeserializeArray<FString>(TEXT("titles"), Bag, Titles, OuterOwner);
	UBeamJsonUtils::DeserializeArray<FString>(TEXT("descriptions"), Bag, Descriptions, OuterOwner);
	UBeamJsonUtils::DeserializeArray<UObtainCurrencyView*>(TEXT("obtainCurrency"), Bag, ObtainCurrency, OuterOwner);
	UBeamJsonUtils::DeserializeArray<UObtainItemsView*>(TEXT("obtainItems"), Bag, ObtainItems, OuterOwner);
}



