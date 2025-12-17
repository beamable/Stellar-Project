
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationGetListingsRequest.h"

void UStellarFederationGetListingsRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationGetListingsRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/GetListings");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationGetListingsRequest::BuildBody(FString& BodyString) const
{
	ensureAlways(Body);

	TUnrealJsonSerializer JsonSerializer = TJsonStringWriter<TCondensedJsonPrintPolicy<TCHAR>>::Create(&BodyString);
	Body->BeamSerialize(JsonSerializer);
	JsonSerializer->Close();
}

UStellarFederationGetListingsRequest* UStellarFederationGetListingsRequest::Make(FString _StoreId, UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationGetListingsRequest* Req = NewObject<UStellarFederationGetListingsRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	Req->Body = NewObject<UGetListingsRequestArgs>(Req);
	Req->Body->StoreId = _StoreId;
	

	return Req;
}
