
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ClientDataViewLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UClientDataViewLibrary::ClientDataViewToJsonString(const UClientDataView* Serializable, const bool Pretty)
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

UClientDataView* UClientDataViewLibrary::Make(FString Name, FString Value, UObject* Outer)
{
	auto Serializable = NewObject<UClientDataView>(Outer);
	Serializable->Name = Name;
	Serializable->Value = Value;
	
	return Serializable;
}

void UClientDataViewLibrary::Break(const UClientDataView* Serializable, FString& Name, FString& Value)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Name = Serializable->Name;
		Value = Serializable->Value;
	}
		
}

