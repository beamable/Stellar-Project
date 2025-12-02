
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/RemoveItemRequestArgs.h"

#include "Misc/DefaultValueHelper.h"



void URemoveItemRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("itemContentId"), ItemContentId, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("instanceId"), InstanceId, Serializer);
}

void URemoveItemRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("itemContentId"), ItemContentId, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("instanceId"), InstanceId, Serializer);		
}

void URemoveItemRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("itemContentId"), Bag, ItemContentId);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("instanceId"), Bag, InstanceId);
}



