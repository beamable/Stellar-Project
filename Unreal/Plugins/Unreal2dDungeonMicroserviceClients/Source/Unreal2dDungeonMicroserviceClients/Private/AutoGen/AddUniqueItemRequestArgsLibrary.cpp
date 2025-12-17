
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AddUniqueItemRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UAddUniqueItemRequestArgsLibrary::AddUniqueItemRequestArgsToJsonString(const UAddUniqueItemRequestArgs* Serializable, const bool Pretty)
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

UAddUniqueItemRequestArgs* UAddUniqueItemRequestArgsLibrary::Make(FString ItemContentId, TMap<FString, FString> Properties, UObject* Outer)
{
	auto Serializable = NewObject<UAddUniqueItemRequestArgs>(Outer);
	Serializable->ItemContentId = ItemContentId;
	Serializable->Properties = Properties;
	
	return Serializable;
}

void UAddUniqueItemRequestArgsLibrary::Break(const UAddUniqueItemRequestArgs* Serializable, FString& ItemContentId, TMap<FString, FString>& Properties)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		ItemContentId = Serializable->ItemContentId;
		Properties = Serializable->Properties;
	}
		
}

