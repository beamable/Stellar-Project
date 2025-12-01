
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateCurrencyRequestArgs.h"

#include "Misc/DefaultValueHelper.h"



void UUpdateCurrencyRequestArgs::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("currencyContentId"), CurrencyContentId, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("amount"), Amount, Serializer);
}

void UUpdateCurrencyRequestArgs::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("currencyContentId"), CurrencyContentId, Serializer);
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("amount"), Amount, Serializer);		
}

void UUpdateCurrencyRequestArgs::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("currencyContentId"), Bag, CurrencyContentId);
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("amount"), Bag, Amount);
}



