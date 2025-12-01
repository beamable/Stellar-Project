
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateCurrencyRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UUpdateCurrencyRequestArgsLibrary::UpdateCurrencyRequestArgsToJsonString(const UUpdateCurrencyRequestArgs* Serializable, const bool Pretty)
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

UUpdateCurrencyRequestArgs* UUpdateCurrencyRequestArgsLibrary::Make(FString CurrencyContentId, int32 Amount, UObject* Outer)
{
	auto Serializable = NewObject<UUpdateCurrencyRequestArgs>(Outer);
	Serializable->CurrencyContentId = CurrencyContentId;
	Serializable->Amount = Amount;
	
	return Serializable;
}

void UUpdateCurrencyRequestArgsLibrary::Break(const UUpdateCurrencyRequestArgs* Serializable, FString& CurrencyContentId, int32& Amount)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		CurrencyContentId = Serializable->CurrencyContentId;
		Amount = Serializable->Amount;
	}
		
}

