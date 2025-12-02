
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/DoOwnWarriorRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UDoOwnWarriorRequestArgsLibrary::DoOwnWarriorRequestArgsToJsonString(const UDoOwnWarriorRequestArgs* Serializable, const bool Pretty)
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

UDoOwnWarriorRequestArgs* UDoOwnWarriorRequestArgsLibrary::Make(FString ItemContentId, UObject* Outer)
{
	auto Serializable = NewObject<UDoOwnWarriorRequestArgs>(Outer);
	Serializable->ItemContentId = ItemContentId;
	
	return Serializable;
}

void UDoOwnWarriorRequestArgsLibrary::Break(const UDoOwnWarriorRequestArgs* Serializable, FString& ItemContentId)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		ItemContentId = Serializable->ItemContentId;
	}
		
}

