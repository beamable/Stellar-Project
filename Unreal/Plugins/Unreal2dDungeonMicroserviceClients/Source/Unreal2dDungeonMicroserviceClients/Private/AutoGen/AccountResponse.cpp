
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AccountResponse.h"




void UAccountResponse::DeserializeRequestResponse(UObject* RequestData, FString ResponseContent)
{
	OuterOwner = RequestData;
	BeamDeserialize(ResponseContent);	
}

void UAccountResponse::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("created"), bCreated, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("wallet"), Wallet, Serializer);
}

void UAccountResponse::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("created"), bCreated, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("wallet"), Wallet, Serializer);		
}

void UAccountResponse::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("created"), Bag, bCreated);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("wallet"), Bag, Wallet);
}



