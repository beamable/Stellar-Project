#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateInventoryRequestArgs.h"

#include "UpdateInventoryRequestArgsLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UUpdateInventoryRequestArgsLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="UpdateInventoryRequestArgs To JSON String")
	static FString UpdateInventoryRequestArgsToJsonString(const UUpdateInventoryRequestArgs* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make UpdateInventoryRequestArgs", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UUpdateInventoryRequestArgs* Make(FString CurrencyContentId, int32 Amount, TArray<UCropUpdateRequestBody*> Items, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break UpdateInventoryRequestArgs", meta=(NativeBreakFunc))
	static void Break(const UUpdateInventoryRequestArgs* Serializable, FString& CurrencyContentId, int32& Amount, TArray<UCropUpdateRequestBody*>& Items);
};