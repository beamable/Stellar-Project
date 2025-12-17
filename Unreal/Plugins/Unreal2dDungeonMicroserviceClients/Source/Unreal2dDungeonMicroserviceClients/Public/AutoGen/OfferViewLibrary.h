#pragma once

#include "CoreMinimal.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/OfferView.h"

#include "OfferViewLibrary.generated.h"


UCLASS(BlueprintType, Category="Beam")
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UOfferViewLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Json", DisplayName="OfferView To JSON String")
	static FString OfferViewToJsonString(const UOfferView* Serializable, const bool Pretty);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make OfferView", meta=(DefaultToSelf="Outer", AdvancedDisplay="Outer", NativeMakeFunc))
	static UOfferView* Make(FString Symbol, UOfferPriceView* Price, TArray<FString> Titles, TArray<FString> Descriptions, TArray<UObtainCurrencyView*> ObtainCurrency, TArray<UObtainItemsView*> ObtainItems, UObject* Outer);

	UFUNCTION(BlueprintPure, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Break OfferView", meta=(NativeBreakFunc))
	static void Break(const UOfferView* Serializable, FString& Symbol, UOfferPriceView*& Price, TArray<FString>& Titles, TArray<FString>& Descriptions, TArray<UObtainCurrencyView*>& ObtainCurrency, TArray<UObtainItemsView*>& ObtainItems);
};