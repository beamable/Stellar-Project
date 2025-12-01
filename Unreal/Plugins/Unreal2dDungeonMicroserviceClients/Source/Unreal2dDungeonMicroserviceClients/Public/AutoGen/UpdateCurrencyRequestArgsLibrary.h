#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateCurrencyRequestArgs.h"

#include "UpdateCurrencyRequestArgsLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UUpdateCurrencyRequestArgsLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="UpdateCurrencyRequestArgs To JSON String")
	static FString UpdateCurrencyRequestArgsToJsonString(const UUpdateCurrencyRequestArgs* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make UpdateCurrencyRequestArgs", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UUpdateCurrencyRequestArgs* Make(FString CurrencyContentId, int32 Amount, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break UpdateCurrencyRequestArgs", meta=(NativeBreakFunc))
	static void Break(const UUpdateCurrencyRequestArgs* Serializable, FString& CurrencyContentId, int32& Amount);
};