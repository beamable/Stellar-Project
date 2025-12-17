#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/CropUpdateRequestBody.h"

#include "CropUpdateRequestBodyLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UCropUpdateRequestBodyLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="CropUpdateRequestBody To JSON String")
	static FString CropUpdateRequestBodyToJsonString(const UCropUpdateRequestBody* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make CropUpdateRequestBody", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UCropUpdateRequestBody* Make(FString ContentId, int64 InstanceId, TMap<FString, FString> Properties, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break CropUpdateRequestBody", meta=(NativeBreakFunc))
	static void Break(const UCropUpdateRequestBody* Serializable, FString& ContentId, int64& InstanceId, TMap<FString, FString>& Properties);
};