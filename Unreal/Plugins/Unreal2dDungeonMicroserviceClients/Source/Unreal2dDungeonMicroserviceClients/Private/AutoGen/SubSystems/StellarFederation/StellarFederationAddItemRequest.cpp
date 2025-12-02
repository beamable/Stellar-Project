
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationAddItemRequest.h"

void UStellarFederationAddItemRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationAddItemRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/AddItem");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationAddItemRequest::BuildBody(FString& BodyString) const
{
	ensureAlways(Body);

	TUnrealJsonSerializer JsonSerializer = TJsonStringWriter<TCondensedJsonPrintPolicy<TCHAR>>::Create(&BodyString);
	Body->BeamSerialize(JsonSerializer);
	JsonSerializer->Close();
}

UStellarFederationAddItemRequest* UStellarFederationAddItemRequest::Make(FString _ItemContentId, TMap<FString, FString> _Properties, UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationAddItemRequest* Req = NewObject<UStellarFederationAddItemRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	Req->Body = NewObject<UAddItemRequestArgs>(Req);
	Req->Body->ItemContentId = _ItemContentId;
	Req->Body->Properties = _Properties;
	

	return Req;
}
