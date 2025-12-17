#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ObtainItemsView.h"

#include "ObtainItemsViewLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UObtainItemsViewLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="ObtainItemsView To JSON String")
	static FString ObtainItemsViewToJsonString(const UObtainItemsView* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make ObtainItemsView", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UObtainItemsView* Make(FString ContentId, TArray<UItemPropertyView*> Properties, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break ObtainItemsView", meta=(NativeBreakFunc))
	static void Break(const UObtainItemsView* Serializable, FString& ContentId, TArray<UItemPropertyView*>& Properties);
};