
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationAddItemResponse.h"
#include "Serialization/BeamJsonUtils.h"



void UStellarFederationAddItemResponse::DeserializeRequestResponse(UObject* RequestData, FString ResponseContent)
{
	OuterOwner = RequestData;
	UBeamJsonUtils::DeserializeRawPrimitive<bool>(ResponseContent, bValue, OuterOwner);
}

void UStellarFederationAddItemResponse::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), bValue, Serializer);
}

void UStellarFederationAddItemResponse::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), bValue, Serializer);		
}

void UStellarFederationAddItemResponse::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("Value"), Bag, bValue);
}



