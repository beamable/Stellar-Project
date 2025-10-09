
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ConfigurationResponse.h"




void UConfigurationResponse::DeserializeRequestResponse(UObject* RequestData, FString ResponseContent)
{
	OuterOwner = RequestData;
	BeamDeserialize(ResponseContent);	
}

void UConfigurationResponse::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("network"), Network, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("walletConnectBridgeUrl"), WalletConnectBridgeUrl, Serializer);
}

void UConfigurationResponse::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("network"), Network, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("walletConnectBridgeUrl"), WalletConnectBridgeUrl, Serializer);		
}

void UConfigurationResponse::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("network"), Bag, Network);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("walletConnectBridgeUrl"), Bag, WalletConnectBridgeUrl);
}



