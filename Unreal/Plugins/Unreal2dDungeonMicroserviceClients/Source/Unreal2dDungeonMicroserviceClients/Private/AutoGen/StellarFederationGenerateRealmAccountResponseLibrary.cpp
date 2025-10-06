
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGenerateRealmAccountResponseLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UStellarFederationGenerateRealmAccountResponseLibrary::StellarFederationGenerateRealmAccountResponseToJsonString(const UStellarFederationGenerateRealmAccountResponse* Serializable, const bool Pretty)
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

UStellarFederationGenerateRealmAccountResponse* UStellarFederationGenerateRealmAccountResponseLibrary::Make(FString Value, UObject* Outer)
{
	auto Serializable = NewObject<UStellarFederationGenerateRealmAccountResponse>(Outer);
	Serializable->Value = Value;
	
	return Serializable;
}

void UStellarFederationGenerateRealmAccountResponseLibrary::Break(const UStellarFederationGenerateRealmAccountResponse* Serializable, FString& Value)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Value = Serializable->Value;
	}
		
}

