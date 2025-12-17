
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/CropUpdateRequestBody.h"
#include "Serialization/BeamJsonUtils.h"
#include "Misc/DefaultValueHelper.h"



void UCropUpdateRequestBody::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("ContentId"), ContentId, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("InstanceId"), InstanceId, Serializer);
	UBeamJsonUtils::SerializeMap<FString>(TEXT("Properties"), Properties, Serializer);
}

void UCropUpdateRequestBody::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("ContentId"), ContentId, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("InstanceId"), InstanceId, Serializer);
	UBeamJsonUtils::SerializeMap<FString>(TEXT("Properties"), Properties, Serializer);		
}

void UCropUpdateRequestBody::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("ContentId"), Bag, ContentId);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("InstanceId"), Bag, InstanceId);
	UBeamJsonUtils::DeserializeMap<FString>(TEXT("Properties"), Bag, Properties, OuterOwner);
}



