
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/JobsRequestArgs.h"





void UJobsRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("enable"), bEnable, Serializer);
}

void UJobsRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("enable"), bEnable, Serializer);		
}

void UJobsRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("enable"), Bag, bEnable);
}



