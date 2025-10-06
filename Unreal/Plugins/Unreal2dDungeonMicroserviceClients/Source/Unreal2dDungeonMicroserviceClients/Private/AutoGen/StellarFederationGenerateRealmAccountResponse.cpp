
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGenerateRealmAccountResponse.h"
#include "Serialization/BeamJsonUtils.h"



void UStellarFederationGenerateRealmAccountResponse::DeserializeRequestResponse(UObject* RequestData, FString ResponseContent)
{
	OuterOwner = RequestData;
	UBeamJsonUtils::DeserializeRawPrimitive<FString>(ResponseContent, Value, OuterOwner);
}

void UStellarFederationGenerateRealmAccountResponse::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), Value, Serializer);
}

void UStellarFederationGenerateRealmAccountResponse::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("Value"), Value, Serializer);		
}

void UStellarFederationGenerateRealmAccountResponse::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("Value"), Bag, Value);
}



