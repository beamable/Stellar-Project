#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/PurchaseWarriorRequestArgs.h"

#include "PurchaseWarriorRequestArgsLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UPurchaseWarriorRequestArgsLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="PurchaseWarriorRequestArgs To JSON String")
	static FString PurchaseWarriorRequestArgsToJsonString(const UPurchaseWarriorRequestArgs* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make PurchaseWarriorRequestArgs", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UPurchaseWarriorRequestArgs* Make(FString ListingId, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break PurchaseWarriorRequestArgs", meta=(NativeBreakFunc))
	static void Break(const UPurchaseWarriorRequestArgs* Serializable, FString& ListingId);
};