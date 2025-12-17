
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ListingViewLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UListingViewLibrary::ListingViewToJsonString(const UListingView* Serializable, const bool Pretty)
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

UListingView* UListingViewLibrary::Make(bool bActive, FString Symbol, int64 SecondsActive, int64 SecondsRemain, int32 PurchasesRemain, int32 Cooldown, UOfferView* Offer, TArray<UClientDataView*> ClientData, UObject* Outer)
{
	auto Serializable = NewObject<UListingView>(Outer);
	Serializable->bActive = bActive;
	Serializable->Symbol = Symbol;
	Serializable->SecondsActive = SecondsActive;
	Serializable->SecondsRemain = SecondsRemain;
	Serializable->PurchasesRemain = PurchasesRemain;
	Serializable->Cooldown = Cooldown;
	Serializable->Offer = Offer;
	Serializable->ClientData = ClientData;
	
	return Serializable;
}

void UListingViewLibrary::Break(const UListingView* Serializable, bool& bActive, FString& Symbol, int64& SecondsActive, int64& SecondsRemain, int32& PurchasesRemain, int32& Cooldown, UOfferView*& Offer, TArray<UClientDataView*>& ClientData)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		bActive = Serializable->bActive;
		Symbol = Serializable->Symbol;
		SecondsActive = Serializable->SecondsActive;
		SecondsRemain = Serializable->SecondsRemain;
		PurchasesRemain = Serializable->PurchasesRemain;
		Cooldown = Serializable->Cooldown;
		Offer = Serializable->Offer;
		ClientData = Serializable->ClientData;
	}
		
}

