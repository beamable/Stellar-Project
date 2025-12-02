
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationAddItemResponseLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UStellarFederationAddItemResponseLibrary::StellarFederationAddItemResponseToJsonString(const UStellarFederationAddItemResponse* Serializable, const bool Pretty)
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

UStellarFederationAddItemResponse* UStellarFederationAddItemResponseLibrary::Make(bool bValue, UObject* Outer)
{
	auto Serializable = NewObject<UStellarFederationAddItemResponse>(Outer);
	Serializable->bValue = bValue;
	
	return Serializable;
}

void UStellarFederationAddItemResponseLibrary::Break(const UStellarFederationAddItemResponse* Serializable, bool& bValue)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		bValue = Serializable->bValue;
	}
		
}

