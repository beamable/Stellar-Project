#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGetRealmAccountResponse.h"

#include "StellarFederationGetRealmAccountResponseLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationGetRealmAccountResponseLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="StellarFederationGetRealmAccountResponse To JSON String")
	static FString StellarFederationGetRealmAccountResponseToJsonString(const UStellarFederationGetRealmAccountResponse* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationGetRealmAccountResponse", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UStellarFederationGetRealmAccountResponse* Make(FString Value, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break StellarFederationGetRealmAccountResponse", meta=(NativeBreakFunc))
	static void Break(const UStellarFederationGetRealmAccountResponse* Serializable, FString& Value);
};