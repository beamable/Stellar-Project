
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateItemsRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UUpdateItemsRequestArgsLibrary::UpdateItemsRequestArgsToJsonString(const UUpdateItemsRequestArgs* Serializable, const bool Pretty)
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

UUpdateItemsRequestArgs* UUpdateItemsRequestArgsLibrary::Make(TArray<UCropUpdateRequestBody*> Items, UObject* Outer)
{
	auto Serializable = NewObject<UUpdateItemsRequestArgs>(Outer);
	Serializable->Items = Items;
	
	return Serializable;
}

void UUpdateItemsRequestArgsLibrary::Break(const UUpdateItemsRequestArgs* Serializable, TArray<UCropUpdateRequestBody*>& Items)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Items = Serializable->Items;
	}
		
}

