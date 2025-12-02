
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/PurchaseWarriorRequestArgs.h"





void UPurchaseWarriorRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("listingId"), ListingId, Serializer);
}

void UPurchaseWarriorRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("listingId"), ListingId, Serializer);		
}

void UPurchaseWarriorRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("listingId"), Bag, ListingId);
}



