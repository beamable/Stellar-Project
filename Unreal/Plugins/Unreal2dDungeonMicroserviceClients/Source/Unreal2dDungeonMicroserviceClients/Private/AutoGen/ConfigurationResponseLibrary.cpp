
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ConfigurationResponseLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UConfigurationResponseLibrary::ConfigurationResponseToJsonString(const UConfigurationResponse* Serializable, const bool Pretty)
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

UConfigurationResponse* UConfigurationResponseLibrary::Make(FString Network, FString WalletConnectBridgeUrl, UObject* Outer)
{
	auto Serializable = NewObject<UConfigurationResponse>(Outer);
	Serializable->Network = Network;
	Serializable->WalletConnectBridgeUrl = WalletConnectBridgeUrl;
	
	return Serializable;
}

void UConfigurationResponseLibrary::Break(const UConfigurationResponse* Serializable, FString& Network, FString& WalletConnectBridgeUrl)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Network = Serializable->Network;
		WalletConnectBridgeUrl = Serializable->WalletConnectBridgeUrl;
	}
		
}

