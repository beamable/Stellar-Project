#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/DoOwnWarriorRequestArgs.h"

#include "DoOwnWarriorRequestArgsLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UDoOwnWarriorRequestArgsLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="DoOwnWarriorRequestArgs To JSON String")
	static FString DoOwnWarriorRequestArgsToJsonString(const UDoOwnWarriorRequestArgs* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make DoOwnWarriorRequestArgs", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UDoOwnWarriorRequestArgs* Make(FString ItemContentId, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break DoOwnWarriorRequestArgs", meta=(NativeBreakFunc))
	static void Break(const UDoOwnWarriorRequestArgs* Serializable, FString& ItemContentId);
};