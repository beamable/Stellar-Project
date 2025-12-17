
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/OfferPriceViewLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UOfferPriceViewLibrary::OfferPriceViewToJsonString(const UOfferPriceView* Serializable, const bool Pretty)
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

UOfferPriceView* UOfferPriceViewLibrary::Make(FString Symbol, FString Type, int32 Amount, TArray<int32> Schedule, UObject* Outer)
{
	auto Serializable = NewObject<UOfferPriceView>(Outer);
	Serializable->Symbol = Symbol;
	Serializable->Type = Type;
	Serializable->Amount = Amount;
	Serializable->Schedule = Schedule;
	
	return Serializable;
}

void UOfferPriceViewLibrary::Break(const UOfferPriceView* Serializable, FString& Symbol, FString& Type, int32& Amount, TArray<int32>& Schedule)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Symbol = Serializable->Symbol;
		Type = Serializable->Type;
		Amount = Serializable->Amount;
		Schedule = Serializable->Schedule;
	}
		
}

