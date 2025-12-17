
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateItemsRequestArgs.h"
#include "Serialization/BeamJsonUtils.h"




void UUpdateItemsRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeArray<UCropUpdateRequestBody*>(TEXT("items"), Items, Serializer);
}

void UUpdateItemsRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeArray<UCropUpdateRequestBody*>(TEXT("items"), Items, Serializer);		
}

void UUpdateItemsRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeArray<UCropUpdateRequestBody*>(TEXT("items"), Bag, Items, OuterOwner);
}



