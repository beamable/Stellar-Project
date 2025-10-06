
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationGetRealmAccountRequest.h"

void UStellarFederationGetRealmAccountRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationGetRealmAccountRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/GetRealmAccount");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationGetRealmAccountRequest::BuildBody(FString& BodyString) const
{
	
}

UStellarFederationGetRealmAccountRequest* UStellarFederationGetRealmAccountRequest::Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationGetRealmAccountRequest* Req = NewObject<UStellarFederationGetRealmAccountRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	

	return Req;
}
