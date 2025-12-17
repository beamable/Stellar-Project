
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SchedulerJobResponse.h"
#include "Serialization/BeamJsonUtils.h"



void USchedulerJobResponse::DeserializeRequestResponse(UObject* RequestData, FString ResponseContent)
{
	OuterOwner = RequestData;
	BeamDeserialize(ResponseContent);	
}

void USchedulerJobResponse::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeArray<FString>(TEXT("names"), Names, Serializer);
}

void USchedulerJobResponse::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeArray<FString>(TEXT("names"), Names, Serializer);		
}

void USchedulerJobResponse::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeArray<FString>(TEXT("names"), Bag, Names, OuterOwner);
}



