
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateInventoryRequestArgs.h"
#include "Serialization/BeamJsonUtils.h"
#include "Misc/DefaultValueHelper.h"



void UUpdateInventoryRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("currencyContentId"), CurrencyContentId, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("amount"), Amount, Serializer);
	UBeamJsonUtils::SerializeArray<UCropUpdateRequestBody*>(TEXT("items"), Items, Serializer);
}

void UUpdateInventoryRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("currencyContentId"), CurrencyContentId, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("amount"), Amount, Serializer);
	UBeamJsonUtils::SerializeArray<UCropUpdateRequestBody*>(TEXT("items"), Items, Serializer);		
}

void UUpdateInventoryRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("currencyContentId"), Bag, CurrencyContentId);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("amount"), Bag, Amount);
	UBeamJsonUtils::DeserializeArray<UCropUpdateRequestBody*>(TEXT("items"), Bag, Items, OuterOwner);
}



