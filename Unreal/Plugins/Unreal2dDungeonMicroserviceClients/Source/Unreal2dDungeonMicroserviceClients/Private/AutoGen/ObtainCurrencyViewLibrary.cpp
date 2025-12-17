
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ObtainCurrencyViewLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UObtainCurrencyViewLibrary::ObtainCurrencyViewToJsonString(const UObtainCurrencyView* Serializable, const bool Pretty)
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

UObtainCurrencyView* UObtainCurrencyViewLibrary::Make(FString Symbol, int64 Amount, UObject* Outer)
{
	auto Serializable = NewObject<UObtainCurrencyView>(Outer);
	Serializable->Symbol = Symbol;
	Serializable->Amount = Amount;
	
	return Serializable;
}

void UObtainCurrencyViewLibrary::Break(const UObtainCurrencyView* Serializable, FString& Symbol, int64& Amount)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Symbol = Serializable->Symbol;
		Amount = Serializable->Amount;
	}
		
}

