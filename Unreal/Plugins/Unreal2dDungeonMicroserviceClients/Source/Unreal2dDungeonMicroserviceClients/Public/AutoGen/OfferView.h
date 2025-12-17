#pragma once

#include "CoreMinimal.h"

#include "Serialization/BeamJsonSerializable.h"
#include "Serialization/BeamJsonUtils.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/OfferPriceView.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ObtainCurrencyView.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ObtainItemsView.h"

#include "OfferView.generated.h"

UCLASS(BlueprintType, Category="Beam", DefaultToInstanced, EditInlineNew)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UOfferView : public UObject, public IBeamJsonSerializableUObject
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Symbol", Category="Beam")
	FString Symbol = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Price", Category="Beam")
	UOfferPriceView* Price = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Titles", Category="Beam")
	TArray<FString> Titles = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Descriptions", Category="Beam")
	TArray<FString> Descriptions = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Obtain Currency", Category="Beam")
	TArray<UObtainCurrencyView*> ObtainCurrency = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Obtain Items", Category="Beam")
	TArray<UObtainItemsView*> ObtainItems = {};

	

	virtual void BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const override;
	virtual void BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const override;
	virtual void BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag) override;
	
};