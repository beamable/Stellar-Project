
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AddItemRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UAddItemRequestArgsLibrary::AddItemRequestArgsToJsonString(const UAddItemRequestArgs* Serializable, const bool Pretty)
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

UAddItemRequestArgs* UAddItemRequestArgsLibrary::Make(FString ItemContentId, TMap<FString, FString> Properties, UObject* Outer)
{
	auto Serializable = NewObject<UAddItemRequestArgs>(Outer);
	Serializable->ItemContentId = ItemContentId;
	Serializable->Properties = Properties;
	
	return Serializable;
}

void UAddItemRequestArgsLibrary::Break(const UAddItemRequestArgs* Serializable, FString& ItemContentId, TMap<FString, FString>& Properties)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		ItemContentId = Serializable->ItemContentId;
		Properties = Serializable->Properties;
	}
		
}

