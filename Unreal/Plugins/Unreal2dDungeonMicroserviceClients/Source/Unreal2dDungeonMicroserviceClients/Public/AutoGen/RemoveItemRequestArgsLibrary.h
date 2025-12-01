#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/RemoveItemRequestArgs.h"

#include "RemoveItemRequestArgsLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API URemoveItemRequestArgsLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="RemoveItemRequestArgs To JSON String")
	static FString RemoveItemRequestArgsToJsonString(const URemoveItemRequestArgs* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make RemoveItemRequestArgs", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static URemoveItemRequestArgs* Make(FString ItemContentId, int64 InstanceId, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break RemoveItemRequestArgs", meta=(NativeBreakFunc))
	static void Break(const URemoveItemRequestArgs* Serializable, FString& ItemContentId, int64& InstanceId);
};