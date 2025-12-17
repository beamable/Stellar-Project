
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SchedulerJobResponseLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString USchedulerJobResponseLibrary::SchedulerJobResponseToJsonString(const USchedulerJobResponse* Serializable, const bool Pretty)
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

USchedulerJobResponse* USchedulerJobResponseLibrary::Make(TArray<FString> Names, UObject* Outer)
{
	auto Serializable = NewObject<USchedulerJobResponse>(Outer);
	Serializable->Names = Names;
	
	return Serializable;
}

void USchedulerJobResponseLibrary::Break(const USchedulerJobResponse* Serializable, TArray<FString>& Names)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		Names = Serializable->Names;
	}
		
}

