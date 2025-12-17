
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/OfferViewLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UOfferViewLibrary::OfferViewToJsonString(const UOfferView* Serializable, const bool Pretty)
{
	FString Result = FString{};
	if(Pretty)
	{
		TUnrealPrettyJsonSerializer JsonSerializer = TJsonStringWriter<TPrettyJsonPrintPolicy<TCHAR>>::Create(&Result);
		Serializable->BeamSerialize(JsonSerializer);
		JsonSerializer->Close();
	}
	else
	{
		TUnrealJsonSerializer JsonSerializer = TJsonStringWriter<TCondensedJsonPrintPolicy<TCHAR>>::Create(&Result);
		Serializable->BeamSerialize(JsonSerializer);
		JsonSerializer->Close();			
	}
	return Result;
}	

UOfferView* UOfferViewLibrary::Make(FString Symbol, UOfferPriceView* Price, TArray<FString> Titles, TArray<FString> Descriptions, TArray<UObtainCurrencyView*> ObtainCurrency, TArray<UObtainItemsView*> ObtainItems, UObject* Outer)
{
	auto Serializable = NewObject<UOfferView>(Outer);
	Serializable->Symbol = Symbol;
	Serializable->Price = Price;
	Serializable->Titles = Titles;
	Serializable->Descriptions = Descriptions;
	Serializable->ObtainCurrency = ObtainCurrency;
	Serializable->ObtainItems = ObtainItems;
	
	return Serializable;
}

void UOfferViewLibrary::Break(const UOfferView* Serializable, FString& Symbol, UOfferPriceView*& Price, TArray<FString>& Titles, TArray<FString>& Descriptions, TArray<UObtainCurrencyView*>& ObtainCurrency, TArray<UObtainItemsView*>& ObtainItems)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Symbol = Serializable->Symbol;
		Price = Serializable->Price;
		Titles = Serializable->Titles;
		Descriptions = Serializable->Descriptions;
		ObtainCurrency = Serializable->ObtainCurrency;
		ObtainItems = Serializable->ObtainItems;
	}
		
}

