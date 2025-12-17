#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationAddUniqueItemResponse.h"

#include "StellarFederationAddUniqueItemResponseLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationAddUniqueItemResponseLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="StellarFederationAddUniqueItemResponse To JSON String")
	static FString StellarFederationAddUniqueItemResponseToJsonString(const UStellarFederationAddUniqueItemResponse* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationAddUniqueItemResponse", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UStellarFederationAddUniqueItemResponse* Make(bool bValue, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break StellarFederationAddUniqueItemResponse", meta=(NativeBreakFunc))
	static void Break(const UStellarFederationAddUniqueItemResponse* Serializable, bool& bValue);
};