
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGetRealmAccountResponse.h"
#include "Serialization/BeamJsonUtils.h"



void UStellarFederationGetRealmAccountResponse::DeserializeRequestResponse(UObject* RequestData, FString ResponseContent)
{
	OuterOwner = RequestData;
	UBeamJsonUtils::DeserializeRawPrimitive<FString>(ResponseContent, Value, OuterOwner);
}

void UStellarFederationGetRealmAccountResponse::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), Value, Serializer);
}

void UStellarFederationGetRealmAccountResponse::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), Value, Serializer);		
}

void UStellarFederationGetRealmAccountResponse::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("Value"), Bag, Value);
}



