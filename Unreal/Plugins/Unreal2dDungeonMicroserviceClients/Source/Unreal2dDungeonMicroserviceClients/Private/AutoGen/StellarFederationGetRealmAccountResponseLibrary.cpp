
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGetRealmAccountResponseLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UStellarFederationGetRealmAccountResponseLibrary::StellarFederationGetRealmAccountResponseToJsonString(const UStellarFederationGetRealmAccountResponse* Serializable, const bool Pretty)
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

UStellarFederationGetRealmAccountResponse* UStellarFederationGetRealmAccountResponseLibrary::Make(FString Value, UObject* Outer)
{
	auto Serializable = NewObject<UStellarFederationGetRealmAccountResponse>(Outer);
	Serializable->Value = Value;
	
	return Serializable;
}

void UStellarFederationGetRealmAccountResponseLibrary::Break(const UStellarFederationGetRealmAccountResponse* Serializable, FString& Value)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Value = Serializable->Value;
	}
		
}

