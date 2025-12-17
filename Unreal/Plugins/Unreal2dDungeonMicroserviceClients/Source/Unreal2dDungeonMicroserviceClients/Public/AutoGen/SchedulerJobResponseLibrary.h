#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SchedulerJobResponse.h"

#include "SchedulerJobResponseLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API USchedulerJobResponseLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="SchedulerJobResponse To JSON String")
	static FString SchedulerJobResponseToJsonString(const USchedulerJobResponse* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make SchedulerJobResponse", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static USchedulerJobResponse* Make(TArray<FString> Names, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break SchedulerJobResponse", meta=(NativeBreakFunc))
	static void Break(const USchedulerJobResponse* Serializable, TArray<FString>& Names);
};