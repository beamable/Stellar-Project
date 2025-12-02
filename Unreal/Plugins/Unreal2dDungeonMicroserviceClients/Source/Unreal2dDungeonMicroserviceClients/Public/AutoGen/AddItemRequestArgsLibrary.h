#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AddItemRequestArgs.h"

#include "AddItemRequestArgsLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UAddItemRequestArgsLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="AddItemRequestArgs To JSON String")
	static FString AddItemRequestArgsToJsonString(const UAddItemRequestArgs* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make AddItemRequestArgs", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UAddItemRequestArgs* Make(FString ItemContentId, TMap<FString, FString> Properties, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break AddItemRequestArgs", meta=(NativeBreakFunc))
	static void Break(const UAddItemRequestArgs* Serializable, FString& ItemContentId, TMap<FString, FString>& Properties);
};