
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationStellarConfigurationRequest.h"

void UStellarFederationStellarConfigurationRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationStellarConfigurationRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/StellarConfiguration");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationStellarConfigurationRequest::BuildBody(FString& BodyString) const
{
	
}

UStellarFederationStellarConfigurationRequest* UStellarFederationStellarConfigurationRequest::Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationStellarConfigurationRequest* Req = NewObject<UStellarFederationStellarConfigurationRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	

	return Req;
}
