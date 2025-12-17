#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateItemsRequestArgs.h"

#include "UpdateItemsRequestArgsLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UUpdateItemsRequestArgsLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="UpdateItemsRequestArgs To JSON String")
	static FString UpdateItemsRequestArgsToJsonString(const UUpdateItemsRequestArgs* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make UpdateItemsRequestArgs", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UUpdateItemsRequestArgs* Make(TArray<UCropUpdateRequestBody*> Items, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break UpdateItemsRequestArgs", meta=(NativeBreakFunc))
	static void Break(const UUpdateItemsRequestArgs* Serializable, TArray<UCropUpdateRequestBody*>& Items);
};