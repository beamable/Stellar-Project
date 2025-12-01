
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AddItemRequestArgs.h"
#include "Serialization/BeamJsonUtils.h"




void UAddItemRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("itemContentId"), ItemContentId, Serializer);
	UBeamJsonUtils::SerializeMap<FString>(TEXT("properties"), Properties, Serializer);
}

void UAddItemRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("itemContentId"), ItemContentId, Serializer);
	UBeamJsonUtils::SerializeMap<FString>(TEXT("properties"), Properties, Serializer);		
}

void UAddItemRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("itemContentId"), Bag, ItemContentId);
	UBeamJsonUtils::DeserializeMap<FString>(TEXT("properties"), Bag, Properties, OuterOwner);
}



