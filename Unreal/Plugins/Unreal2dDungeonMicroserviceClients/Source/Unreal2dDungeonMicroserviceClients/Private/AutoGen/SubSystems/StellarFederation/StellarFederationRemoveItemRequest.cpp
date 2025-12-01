
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationRemoveItemRequest.h"

void UStellarFederationRemoveItemRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationRemoveItemRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/RemoveItem");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationRemoveItemRequest::BuildBody(FString& BodyString) const
{
	ensureAlways(Body);

	TUnrealJsonSerializer JsonSerializer = TJsonStringWriter<TCondensedJsonPrintPolicy<TCHAR>>::Create(&BodyString);
	Body->BeamSerialize(JsonSerializer);
	JsonSerializer->Close();
}

UStellarFederationRemoveItemRequest* UStellarFederationRemoveItemRequest::Make(FString _ItemContentId, int64 _InstanceId, UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationRemoveItemRequest* Req = NewObject<UStellarFederationRemoveItemRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	Req->Body = NewObject<URemoveItemRequestArgs>(Req);
	Req->Body->ItemContentId = _ItemContentId;
	Req->Body->InstanceId = _InstanceId;
	

	return Req;
}
