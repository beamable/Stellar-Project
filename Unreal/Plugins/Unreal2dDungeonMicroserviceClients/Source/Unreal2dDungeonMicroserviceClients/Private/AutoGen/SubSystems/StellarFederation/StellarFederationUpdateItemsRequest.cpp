
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationUpdateItemsRequest.h"

void UStellarFederationUpdateItemsRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationUpdateItemsRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/UpdateItems");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationUpdateItemsRequest::BuildBody(FString& BodyString) const
{
	ensureAlways(Body);

	TUnrealJsonSerializer JsonSerializer = TJsonStringWriter<TCondensedJsonPrintPolicy<TCHAR>>::Create(&BodyString);
	Body->BeamSerialize(JsonSerializer);
	JsonSerializer->Close();
}

UStellarFederationUpdateItemsRequest* UStellarFederationUpdateItemsRequest::Make(TArray<UCropUpdateRequestBody*> _Items, UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationUpdateItemsRequest* Req = NewObject<UStellarFederationUpdateItemsRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	Req->Body = NewObject<UUpdateItemsRequestArgs>(Req);
	Req->Body->Items = _Items;
	

	return Req;
}
