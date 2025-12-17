
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationCreateAccountRequest.h"

void UStellarFederationCreateAccountRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationCreateAccountRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/CreateAccount");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationCreateAccountRequest::BuildBody(FString& BodyString) const
{
	
}

UStellarFederationCreateAccountRequest* UStellarFederationCreateAccountRequest::Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationCreateAccountRequest* Req = NewObject<UStellarFederationCreateAccountRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	

	return Req;
}
