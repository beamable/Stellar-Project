#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StoreView.h"

#include "StoreViewLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStoreViewLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="StoreView To JSON String")
	static FString StoreViewToJsonString(const UStoreView* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StoreView", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UStoreView* Make(FString Title, FString Symbol, int64 NextDeltaSeconds, int64 SecondsRemain, TArray<UListingView*> Listings, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break StoreView", meta=(NativeBreakFunc))
	static void Break(const UStoreView* Serializable, FString& Title, FString& Symbol, int64& NextDeltaSeconds, int64& SecondsRemain, TArray<UListingView*>& Listings);
};