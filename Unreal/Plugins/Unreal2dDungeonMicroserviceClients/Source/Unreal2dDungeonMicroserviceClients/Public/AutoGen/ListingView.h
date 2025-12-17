#pragma once

#include "CoreMinimal.h"

#include "Serialization/BeamJsonSerializable.h"
#include "Serialization/BeamJsonUtils.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ClientDataView.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/OfferView.h"

#include "ListingView.generated.h"

UCLASS(BlueprintType, Category="Beam", DefaultToInstanced, EditInlineNew)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UListingView : public UObject, public IBeamJsonSerializableUObject
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="B Active", Category="Beam")
	bool bActive = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Symbol", Category="Beam")
	FString Symbol = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Seconds Active", Category="Beam")
	int64 SecondsActive = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Seconds Remain", Category="Beam")
	int64 SecondsRemain = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Purchases Remain", Category="Beam")
	int32 PurchasesRemain = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Cooldown", Category="Beam")
	int32 Cooldown = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Offer", Category="Beam")
	UOfferView* Offer = {};
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="Client Data", Category="Beam")
	TArray<UClientDataView*> ClientData = {};

	

	virtual void BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const override;
	virtual void BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const override;
	virtual void BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag) override;
	
};