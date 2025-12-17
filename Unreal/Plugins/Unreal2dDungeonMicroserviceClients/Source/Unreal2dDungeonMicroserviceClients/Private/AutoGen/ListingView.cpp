
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ListingView.h"
#include "Serialization/BeamJsonUtils.h"
#include "Misc/DefaultValueHelper.h"



void UListingView::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("active"), bActive, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("symbol"), Symbol, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("secondsActive"), SecondsActive, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("secondsRemain"), SecondsRemain, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("purchasesRemain"), PurchasesRemain, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("cooldown"), Cooldown, Serializer);
	UBeamJsonUtils::SerializeUObject<UOfferView*>("offer", Offer, Serializer);
	UBeamJsonUtils::SerializeArray<UClientDataView*>(TEXT("clientData"), ClientData, Serializer);
}

void UListingView::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("active"), bActive, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("symbol"), Symbol, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("secondsActive"), SecondsActive, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("secondsRemain"), SecondsRemain, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("purchasesRemain"), PurchasesRemain, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("cooldown"), Cooldown, Serializer);
	UBeamJsonUtils::SerializeUObject<UOfferView*>("offer", Offer, Serializer);
	UBeamJsonUtils::SerializeArray<UClientDataView*>(TEXT("clientData"), ClientData, Serializer);		
}

void UListingView::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("active"), Bag, bActive);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("symbol"), Bag, Symbol);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("secondsActive"), Bag, SecondsActive);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("secondsRemain"), Bag, SecondsRemain);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("purchasesRemain"), Bag, PurchasesRemain);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("cooldown"), Bag, Cooldown);
	UBeamJsonUtils::DeserializeUObject<UOfferView*>("offer", Bag, Offer, OuterOwner);
	UBeamJsonUtils::DeserializeArray<UClientDataView*>(TEXT("clientData"), Bag, ClientData, OuterOwner);
}



