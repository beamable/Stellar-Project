
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/JobsRequestArgsLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UJobsRequestArgsLibrary::JobsRequestArgsToJsonString(const UJobsRequestArgs* Serializable, const bool Pretty)
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

UJobsRequestArgs* UJobsRequestArgsLibrary::Make(bool bEnable, UObject* Outer)
{
	auto Serializable = NewObject<UJobsRequestArgs>(Outer);
	Serializable->bEnable = bEnable;
	
	return Serializable;
}

void UJobsRequestArgsLibrary::Break(const UJobsRequestArgs* Serializable, bool& bEnable)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		bEnable = Serializable->bEnable;
	}
		
}

