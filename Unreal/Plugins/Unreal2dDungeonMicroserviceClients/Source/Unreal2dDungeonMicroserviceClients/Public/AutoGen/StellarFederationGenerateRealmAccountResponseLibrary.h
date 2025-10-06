#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGenerateRealmAccountResponse.h"

#include "StellarFederationGenerateRealmAccountResponseLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationGenerateRealmAccountResponseLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="StellarFederationGenerateRealmAccountResponse To JSON String")
	static FString StellarFederationGenerateRealmAccountResponseToJsonString(const UStellarFederationGenerateRealmAccountResponse* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationGenerateRealmAccountResponse", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UStellarFederationGenerateRealmAccountResponse* Make(FString Value, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break StellarFederationGenerateRealmAccountResponse", meta=(NativeBreakFunc))
	static void Break(const UStellarFederationGenerateRealmAccountResponse* Serializable, FString& Value);
};