
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationGetAccountRequest.h"

void UStellarFederationGetAccountRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationGetAccountRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/GetAccount");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationGetAccountRequest::BuildBody(FString& BodyString) const
{
	
}

UStellarFederationGetAccountRequest* UStellarFederationGetAccountRequest::Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationGetAccountRequest* Req = NewObject<UStellarFederationGetAccountRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	

	return Req;
}
