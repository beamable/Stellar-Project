
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/GetListingsRequestArgs.h"





void UGetListingsRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("storeId"), StoreId, Serializer);
}

void UGetListingsRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("storeId"), StoreId, Serializer);		
}

void UGetListingsRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("storeId"), Bag, StoreId);
}



