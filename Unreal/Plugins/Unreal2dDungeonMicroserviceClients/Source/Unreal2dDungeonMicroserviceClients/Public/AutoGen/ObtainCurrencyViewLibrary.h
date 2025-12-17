#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ObtainCurrencyView.h"

#include "ObtainCurrencyViewLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UObtainCurrencyViewLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="ObtainCurrencyView To JSON String")
	static FString ObtainCurrencyViewToJsonString(const UObtainCurrencyView* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make ObtainCurrencyView", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UObtainCurrencyView* Make(FString Symbol, int64 Amount, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break ObtainCurrencyView", meta=(NativeBreakFunc))
	static void Break(const UObtainCurrencyView* Serializable, FString& Symbol, int64& Amount);
};