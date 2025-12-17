
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationAddUniqueItemResponseLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UStellarFederationAddUniqueItemResponseLibrary::StellarFederationAddUniqueItemResponseToJsonString(const UStellarFederationAddUniqueItemResponse* Serializable, const bool Pretty)
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

UStellarFederationAddUniqueItemResponse* UStellarFederationAddUniqueItemResponseLibrary::Make(bool bValue, UObject* Outer)
{
	auto Serializable = NewObject<UStellarFederationAddUniqueItemResponse>(Outer);
	Serializable->bValue = bValue;
	
	return Serializable;
}

void UStellarFederationAddUniqueItemResponseLibrary::Break(const UStellarFederationAddUniqueItemResponse* Serializable, bool& bValue)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		bValue = Serializable->bValue;
	}
		
}

