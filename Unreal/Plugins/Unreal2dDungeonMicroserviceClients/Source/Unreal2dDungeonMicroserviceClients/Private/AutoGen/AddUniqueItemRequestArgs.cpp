
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AddUniqueItemRequestArgs.h"
#include "Serialization/BeamJsonUtils.h"




void UAddUniqueItemRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("itemContentId"), ItemContentId, Serializer);
	UBeamJsonUtils::SerializeMap<FString>(TEXT("properties"), Properties, Serializer);
}

void UAddUniqueItemRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("itemContentId"), ItemContentId, Serializer);
	UBeamJsonUtils::SerializeMap<FString>(TEXT("properties"), Properties, Serializer);		
}

void UAddUniqueItemRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("itemContentId"), Bag, ItemContentId);
	UBeamJsonUtils::DeserializeMap<FString>(TEXT("properties"), Bag, Properties, OuterOwner);
}



