#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ConfigurationResponse.h"

#include "ConfigurationResponseLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UConfigurationResponseLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="ConfigurationResponse To JSON String")
	static FString ConfigurationResponseToJsonString(const UConfigurationResponse* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make ConfigurationResponse", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UConfigurationResponse* Make(FString Network, FString WalletConnectBridgeUrl, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break ConfigurationResponse", meta=(NativeBreakFunc))
	static void Break(const UConfigurationResponse* Serializable, FString& Network, FString& WalletConnectBridgeUrl);
};