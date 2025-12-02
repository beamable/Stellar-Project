
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/PurchaseWarriorRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UPurchaseWarriorRequestArgsLibrary::PurchaseWarriorRequestArgsToJsonString(const UPurchaseWarriorRequestArgs* Serializable, const bool Pretty)
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

UPurchaseWarriorRequestArgs* UPurchaseWarriorRequestArgsLibrary::Make(FString ListingId, UObject* Outer)
{
	auto Serializable = NewObject<UPurchaseWarriorRequestArgs>(Outer);
	Serializable->ListingId = ListingId;
	
	return Serializable;
}

void UPurchaseWarriorRequestArgsLibrary::Break(const UPurchaseWarriorRequestArgs* Serializable, FString& ListingId)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		ListingId = Serializable->ListingId;
	}
		
}

