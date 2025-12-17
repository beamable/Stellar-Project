
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StoreView.h"
#include "Serialization/BeamJsonUtils.h"
#include "Misc/DefaultValueHelper.h"



void UStoreView::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("title"), Title, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("symbol"), Symbol, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("nextDeltaSeconds"), NextDeltaSeconds, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("secondsRemain"), SecondsRemain, Serializer);
	UBeamJsonUtils::SerializeArray<UListingView*>(TEXT("listings"), Listings, Serializer);
}

void UStoreView::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("title"), Title, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("symbol"), Symbol, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("nextDeltaSeconds"), NextDeltaSeconds, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("secondsRemain"), SecondsRemain, Serializer);
	UBeamJsonUtils::SerializeArray<UListingView*>(TEXT("listings"), Listings, Serializer);		
}

void UStoreView::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("title"), Bag, Title);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("symbol"), Bag, Symbol);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("nextDeltaSeconds"), Bag, NextDeltaSeconds);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("secondsRemain"), Bag, SecondsRemain);
	UBeamJsonUtils::DeserializeArray<UListingView*>(TEXT("listings"), Bag, Listings, OuterOwner);
}



