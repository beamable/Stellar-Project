#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationDoOwnWarriorResponse.h"

#include "StellarFederationDoOwnWarriorResponseLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationDoOwnWarriorResponseLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="StellarFederationDoOwnWarriorResponse To JSON String")
	static FString StellarFederationDoOwnWarriorResponseToJsonString(const UStellarFederationDoOwnWarriorResponse* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationDoOwnWarriorResponse", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UStellarFederationDoOwnWarriorResponse* Make(bool bValue, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break StellarFederationDoOwnWarriorResponse", meta=(NativeBreakFunc))
	static void Break(const UStellarFederationDoOwnWarriorResponse* Serializable, bool& bValue);
};