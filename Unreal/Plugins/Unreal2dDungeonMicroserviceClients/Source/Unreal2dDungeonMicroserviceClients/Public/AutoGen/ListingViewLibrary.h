#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ListingView.h"

#include "ListingViewLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UListingViewLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="ListingView To JSON String")
	static FString ListingViewToJsonString(const UListingView* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make ListingView", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UListingView* Make(bool bActive, FString Symbol, int64 SecondsActive, int64 SecondsRemain, int32 PurchasesRemain, int32 Cooldown, UOfferView* Offer, TArray<UClientDataView*> ClientData, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break ListingView", meta=(NativeBreakFunc))
	static void Break(const UListingView* Serializable, bool& bActive, FString& Symbol, int64& SecondsActive, int64& SecondsRemain, int32& PurchasesRemain, int32& Cooldown, UOfferView*& Offer, TArray<UClientDataView*>& ClientData);
};