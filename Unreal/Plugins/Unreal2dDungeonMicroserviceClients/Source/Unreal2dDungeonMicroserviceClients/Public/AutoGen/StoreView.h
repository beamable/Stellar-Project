#pragma once

#include "CoreMinimal.h"

#include "Serialization/BeamJsonSerializable.h"
#include "Serialization/BeamJsonUtils.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ListingView.h"

#include "StoreView.generated.h"

UCLASS(BlueprintType, Category="Beam", DefaultToInstanced, EditInlineNew)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStoreView : public UObject, public IBeamJsonSerializableUObject
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Title", Category="Beam")
	FString Title = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Symbol", Category="Beam")
	FString Symbol = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Next Delta Seconds", Category="Beam")
	int64 NextDeltaSeconds = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Seconds Remain", Category="Beam")
	int64 SecondsRemain = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Listings", Category="Beam")
	TArray<UListingView*> Listings = {};

	

	virtual void BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const override;
	virtual void BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const override;
	virtual void BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag) override;
	
};