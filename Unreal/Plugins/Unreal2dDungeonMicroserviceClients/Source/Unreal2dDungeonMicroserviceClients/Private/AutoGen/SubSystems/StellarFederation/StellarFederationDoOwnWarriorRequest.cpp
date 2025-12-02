
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationDoOwnWarriorRequest.h"

void UStellarFederationDoOwnWarriorRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationDoOwnWarriorRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/DoOwnWarrior");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationDoOwnWarriorRequest::BuildBody(FString& BodyString) const
{
	ensureAlways(Body);

	TUnrealJsonSerializer JsonSerializer = TJsonStringWriter<TCondensedJsonPrintPolicy<TCHAR>>::Create(&BodyString);
	Body->BeamSerialize(JsonSerializer);
	JsonSerializer->Close();
}

UStellarFederationDoOwnWarriorRequest* UStellarFederationDoOwnWarriorRequest::Make(FString _ItemContentId, UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationDoOwnWarriorRequest* Req = NewObject<UStellarFederationDoOwnWarriorRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	Req->Body = NewObject<UDoOwnWarriorRequestArgs>(Req);
	Req->Body->ItemContentId = _ItemContentId;
	

	return Req;
}
