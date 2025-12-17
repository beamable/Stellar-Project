
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/CropUpdateRequestBodyLibrary.h"

#include "CoreMinimal.h"
#include "BeamCoreSettings.h"


FString UCropUpdateRequestBodyLibrary::CropUpdateRequestBodyToJsonString(const UCropUpdateRequestBody* Serializable, const bool Pretty)
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

UCropUpdateRequestBody* UCropUpdateRequestBodyLibrary::Make(FString ContentId, int64 InstanceId, TMap<FString, FString> Properties, UObject* Outer)
{
	auto Serializable = NewObject<UCropUpdateRequestBody>(Outer);
	Serializable->ContentId = ContentId;
	Serializable->InstanceId = InstanceId;
	Serializable->Properties = Properties;
	
	return Serializable;
}

void UCropUpdateRequestBodyLibrary::Break(const UCropUpdateRequestBody* Serializable, FString& ContentId, int64& InstanceId, TMap<FString, FString>& Properties)
{
	if(GetDefault<UBeamCoreSettings>()->BreakGuard(Serializable))
	{
		ContentId = Serializable->ContentId;
		InstanceId = Serializable->InstanceId;
		Properties = Serializable->Properties;
	}
		
}

