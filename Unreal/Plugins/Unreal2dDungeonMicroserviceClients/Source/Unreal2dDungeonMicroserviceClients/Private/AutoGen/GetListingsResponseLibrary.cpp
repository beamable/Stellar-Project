
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/GetListingsResponseLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UGetListingsResponseLibrary::GetListingsResponseToJsonString(const UGetListingsResponse* Serializable, const bool Pretty)
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

UGetListingsResponse* UGetListingsResponseLibrary::Make(TArray<UStoreView*> Stores, UObject* Outer)
{
	auto Serializable = NewObject<UGetListingsResponse>(Outer);
	Serializable->Stores = Stores;
	
	return Serializable;
}

void UGetListingsResponseLibrary::Break(const UGetListingsResponse* Serializable, TArray<UStoreView*>& Stores)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Stores = Serializable->Stores;
	}
		
}

