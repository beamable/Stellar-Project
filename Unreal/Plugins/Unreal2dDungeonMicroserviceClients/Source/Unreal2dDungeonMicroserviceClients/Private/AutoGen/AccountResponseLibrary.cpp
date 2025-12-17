
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AccountResponseLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UAccountResponseLibrary::AccountResponseToJsonString(const UAccountResponse* Serializable, const bool Pretty)
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

UAccountResponse* UAccountResponseLibrary::Make(bool bCreated, FString Wallet, UObject* Outer)
{
	auto Serializable = NewObject<UAccountResponse>(Outer);
	Serializable->bCreated = bCreated;
	Serializable->Wallet = Wallet;
	
	return Serializable;
}

void UAccountResponseLibrary::Break(const UAccountResponse* Serializable, bool& bCreated, FString& Wallet)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		bCreated = Serializable->bCreated;
		Wallet = Serializable->Wallet;
	}
		
}

