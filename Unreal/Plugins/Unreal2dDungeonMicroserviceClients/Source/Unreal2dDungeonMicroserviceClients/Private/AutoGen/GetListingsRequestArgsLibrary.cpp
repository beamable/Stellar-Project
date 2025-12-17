
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/GetListingsRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UGetListingsRequestArgsLibrary::GetListingsRequestArgsToJsonString(const UGetListingsRequestArgs* Serializable, const bool Pretty)
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

UGetListingsRequestArgs* UGetListingsRequestArgsLibrary::Make(FString StoreId, UObject* Outer)
{
	auto Serializable = NewObject<UGetListingsRequestArgs>(Outer);
	Serializable->StoreId = StoreId;
	
	return Serializable;
}

void UGetListingsRequestArgsLibrary::Break(const UGetListingsRequestArgs* Serializable, FString& StoreId)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		StoreId = Serializable->StoreId;
	}
		
}

