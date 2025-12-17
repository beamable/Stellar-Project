#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/GetListingsResponse.h"

#include "GetListingsResponseLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UGetListingsResponseLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="GetListingsResponse To JSON String")
	static FString GetListingsResponseToJsonString(const UGetListingsResponse* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make GetListingsResponse", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UGetListingsResponse* Make(TArray<UStoreView*> Stores, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break GetListingsResponse", meta=(NativeBreakFunc))
	static void Break(const UGetListingsResponse* Serializable, TArray<UStoreView*>& Stores);
};