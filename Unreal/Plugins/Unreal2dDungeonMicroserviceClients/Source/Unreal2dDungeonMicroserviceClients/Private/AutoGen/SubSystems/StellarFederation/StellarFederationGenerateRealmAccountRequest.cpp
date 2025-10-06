
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationGenerateRealmAccountRequest.h"

void UStellarFederationGenerateRealmAccountRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationGenerateRealmAccountRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/GenerateRealmAccount");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationGenerateRealmAccountRequest::BuildBody(FString& BodyString) const
{
	
}

UStellarFederationGenerateRealmAccountRequest* UStellarFederationGenerateRealmAccountRequest::Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationGenerateRealmAccountRequest* Req = NewObject<UStellarFederationGenerateRealmAccountRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	

	return Req;
}
