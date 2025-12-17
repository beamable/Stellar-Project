#pragma once

#include "CoreMinimal.h"

#include "Serialization/BeamJsonSerializable.h"
#include "Serialization/BeamJsonUtils.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/CropUpdateRequestBody.h"

#include "UpdateInventoryRequestArgs.generated.h"

UCLASS(BlueprintType, Category="Beam", DefaultToInstanced, EditInlineNew)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UUpdateInventoryRequestArgs : public UObject, public IBeamJsonSerializableUObject
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Currency Content Id", Category="Beam")
	FString CurrencyContentId = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Amount", Category="Beam")
	int32 Amount = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Items", Category="Beam")
	TArray<UCropUpdateRequestBody*> Items = {};

	

	virtual void BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const override;
	virtual void BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const override;
	virtual void BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag) override;
	
};