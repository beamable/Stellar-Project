
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/RemoveItemRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString URemoveItemRequestArgsLibrary::RemoveItemRequestArgsToJsonString(const URemoveItemRequestArgs* Serializable, const bool Pretty)
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

URemoveItemRequestArgs* URemoveItemRequestArgsLibrary::Make(FString ItemContentId, int64 InstanceId, UObject* Outer)
{
	auto Serializable = NewObject<URemoveItemRequestArgs>(Outer);
	Serializable->ItemContentId = ItemContentId;
	Serializable->InstanceId = InstanceId;
	
	return Serializable;
}

void URemoveItemRequestArgsLibrary::Break(const URemoveItemRequestArgs* Serializable, FString& ItemContentId, int64& InstanceId)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		ItemContentId = Serializable->ItemContentId;
		InstanceId = Serializable->InstanceId;
	}
		
}

