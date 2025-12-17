
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationAddUniqueItemResponse.h"
#include "Serialization/BeamJsonUtils.h"



void UStellarFederationAddUniqueItemResponse::DeserializeRequestResponse(UObject* RequestData, FString ResponseContent)
{
	OuterOwner = RequestData;
	UBeamJsonUtils::DeserializeRawPrimitive<bool>(ResponseContent, bValue, OuterOwner);
}

void UStellarFederationAddUniqueItemResponse::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), bValue, Serializer);
}

void UStellarFederationAddUniqueItemResponse::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), bValue, Serializer);		
}

void UStellarFederationAddUniqueItemResponse::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("Value"), Bag, bValue);
}



