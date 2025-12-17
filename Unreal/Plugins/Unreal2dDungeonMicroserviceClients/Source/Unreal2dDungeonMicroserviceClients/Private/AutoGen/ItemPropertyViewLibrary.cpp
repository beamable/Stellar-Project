
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ItemPropertyViewLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UItemPropertyViewLibrary::ItemPropertyViewToJsonString(const UItemPropertyView* Serializable, const bool Pretty)
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

UItemPropertyView* UItemPropertyViewLibrary::Make(FString Name, FString Value, UObject* Outer)
{
	auto Serializable = NewObject<UItemPropertyView>(Outer);
	Serializable->Name = Name;
	Serializable->Value = Value;
	
	return Serializable;
}

void UItemPropertyViewLibrary::Break(const UItemPropertyView* Serializable, FString& Name, FString& Value)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Name = Serializable->Name;
		Value = Serializable->Value;
	}
		
}

