
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StoreViewLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UStoreViewLibrary::StoreViewToJsonString(const UStoreView* Serializable, const bool Pretty)
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

UStoreView* UStoreViewLibrary::Make(FString Title, FString Symbol, int64 NextDeltaSeconds, int64 SecondsRemain, TArray<UListingView*> Listings, UObject* Outer)
{
	auto Serializable = NewObject<UStoreView>(Outer);
	Serializable->Title = Title;
	Serializable->Symbol = Symbol;
	Serializable->NextDeltaSeconds = NextDeltaSeconds;
	Serializable->SecondsRemain = SecondsRemain;
	Serializable->Listings = Listings;
	
	return Serializable;
}

void UStoreViewLibrary::Break(const UStoreView* Serializable, FString& Title, FString& Symbol, int64& NextDeltaSeconds, int64& SecondsRemain, TArray<UListingView*>& Listings)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Title = Serializable->Title;
		Symbol = Serializable->Symbol;
		NextDeltaSeconds = Serializable->NextDeltaSeconds;
		SecondsRemain = Serializable->SecondsRemain;
		Listings = Serializable->Listings;
	}
		
}

