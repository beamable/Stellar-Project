
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/GetListingsResponse.h"
#include "Serialization/BeamJsonUtils.h"



void UGetListingsResponse::DeserializeRequestResponse(UObject* RequestData, FString ResponseContent)
{
	OuterOwner = RequestData;
	BeamDeserialize(ResponseContent);	
}

void UGetListingsResponse::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeArray<UStoreView*>(TEXT("stores"), Stores, Serializer);
}

void UGetListingsResponse::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeArray<UStoreView*>(TEXT("stores"), Stores, Serializer);		
}

void UGetListingsResponse::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeArray<UStoreView*>(TEXT("stores"), Bag, Stores, OuterOwner);
}



