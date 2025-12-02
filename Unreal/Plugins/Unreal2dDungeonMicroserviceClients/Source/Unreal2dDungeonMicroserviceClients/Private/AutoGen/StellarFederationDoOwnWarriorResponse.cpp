
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationDoOwnWarriorResponse.h"
#include "Serialization/BeamJsonUtils.h"



void UStellarFederationDoOwnWarriorResponse::DeserializeRequestResponse(UObject* RequestData, FString ResponseContent)
{
	OuterOwner = RequestData;
	UBeamJsonUtils::DeserializeRawPrimitive<bool>(ResponseContent, bValue, OuterOwner);
}

void UStellarFederationDoOwnWarriorResponse::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), bValue, Serializer);
}

void UStellarFederationDoOwnWarriorResponse::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), bValue, Serializer);		
}

void UStellarFederationDoOwnWarriorResponse::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("Value"), Bag, bValue);
}



