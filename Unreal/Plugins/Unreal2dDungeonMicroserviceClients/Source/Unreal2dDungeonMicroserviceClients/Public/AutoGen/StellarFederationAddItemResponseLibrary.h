#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationAddItemResponse.h"

#include "StellarFederationAddItemResponseLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationAddItemResponseLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="StellarFederationAddItemResponse To JSON String")
	static FString StellarFederationAddItemResponseToJsonString(const UStellarFederationAddItemResponse* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationAddItemResponse", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UStellarFederationAddItemResponse* Make(bool bValue, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break StellarFederationAddItemResponse", meta=(NativeBreakFunc))
	static void Break(const UStellarFederationAddItemResponse* Serializable, bool& bValue);
};