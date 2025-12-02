
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/DoOwnWarriorRequestArgs.h"





void UDoOwnWarriorRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("itemContentId"), ItemContentId, Serializer);
}

void UDoOwnWarriorRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("itemContentId"), ItemContentId, Serializer);		
}

void UDoOwnWarriorRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("itemContentId"), Bag, ItemContentId);
}



