
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationDoOwnWarriorResponseLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UStellarFederationDoOwnWarriorResponseLibrary::StellarFederationDoOwnWarriorResponseToJsonString(const UStellarFederationDoOwnWarriorResponse* Serializable, const bool Pretty)
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

UStellarFederationDoOwnWarriorResponse* UStellarFederationDoOwnWarriorResponseLibrary::Make(bool bValue, UObject* Outer)
{
	auto Serializable = NewObject<UStellarFederationDoOwnWarriorResponse>(Outer);
	Serializable->bValue = bValue;
	
	return Serializable;
}

void UStellarFederationDoOwnWarriorResponseLibrary::Break(const UStellarFederationDoOwnWarriorResponse* Serializable, bool& bValue)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		bValue = Serializable->bValue;
	}
		
}

