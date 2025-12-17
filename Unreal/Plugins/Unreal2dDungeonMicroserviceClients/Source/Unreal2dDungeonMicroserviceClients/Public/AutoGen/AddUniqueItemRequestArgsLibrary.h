#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AddUniqueItemRequestArgs.h"

#include "AddUniqueItemRequestArgsLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UAddUniqueItemRequestArgsLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="AddUniqueItemRequestArgs To JSON String")
	static FString AddUniqueItemRequestArgsToJsonString(const UAddUniqueItemRequestArgs* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make AddUniqueItemRequestArgs", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UAddUniqueItemRequestArgs* Make(FString ItemContentId, TMap<FString, FString> Properties, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break AddUniqueItemRequestArgs", meta=(NativeBreakFunc))
	static void Break(const UAddUniqueItemRequestArgs* Serializable, FString& ItemContentId, TMap<FString, FString>& Properties);
};