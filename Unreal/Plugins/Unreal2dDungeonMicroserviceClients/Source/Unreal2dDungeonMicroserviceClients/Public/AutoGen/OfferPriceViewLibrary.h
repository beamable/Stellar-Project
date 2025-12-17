#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/OfferPriceView.h"

#include "OfferPriceViewLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UOfferPriceViewLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="OfferPriceView To JSON String")
	static FString OfferPriceViewToJsonString(const UOfferPriceView* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make OfferPriceView", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UOfferPriceView* Make(FString Symbol, FString Type, int32 Amount, TArray<int32> Schedule, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break OfferPriceView", meta=(NativeBreakFunc))
	static void Break(const UOfferPriceView* Serializable, FString& Symbol, FString& Type, int32& Amount, TArray<int32>& Schedule);
};