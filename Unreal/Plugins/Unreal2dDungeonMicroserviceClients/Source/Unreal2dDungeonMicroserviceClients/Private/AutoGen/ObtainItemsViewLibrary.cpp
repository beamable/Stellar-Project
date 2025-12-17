
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ObtainItemsViewLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UObtainItemsViewLibrary::ObtainItemsViewToJsonString(const UObtainItemsView* Serializable, const bool Pretty)
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

UObtainItemsView* UObtainItemsViewLibrary::Make(FString ContentId, TArray<UItemPropertyView*> Properties, UObject* Outer)
{
	auto Serializable = NewObject<UObtainItemsView>(Outer);
	Serializable->ContentId = ContentId;
	Serializable->Properties = Properties;
	
	return Serializable;
}

void UObtainItemsViewLibrary::Break(const UObtainItemsView* Serializable, FString& ContentId, TArray<UItemPropertyView*>& Properties)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		ContentId = Serializable->ContentId;
		Properties = Serializable->Properties;
	}
		
}

