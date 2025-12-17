
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateInventoryRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UUpdateInventoryRequestArgsLibrary::UpdateInventoryRequestArgsToJsonString(const UUpdateInventoryRequestArgs* Serializable, const bool Pretty)
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

UUpdateInventoryRequestArgs* UUpdateInventoryRequestArgsLibrary::Make(FString CurrencyContentId, int32 Amount, TArray<UCropUpdateRequestBody*> Items, UObject* Outer)
{
	auto Serializable = NewObject<UUpdateInventoryRequestArgs>(Outer);
	Serializable->CurrencyContentId = CurrencyContentId;
	Serializable->Amount = Amount;
	Serializable->Items = Items;
	
	return Serializable;
}

void UUpdateInventoryRequestArgsLibrary::Break(const UUpdateInventoryRequestArgs* Serializable, FString& CurrencyContentId, int32& Amount, TArray<UCropUpdateRequestBody*>& Items)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		CurrencyContentId = Serializable->CurrencyContentId;
		Amount = Serializable->Amount;
		Items = Serializable->Items;
	}
		
}

