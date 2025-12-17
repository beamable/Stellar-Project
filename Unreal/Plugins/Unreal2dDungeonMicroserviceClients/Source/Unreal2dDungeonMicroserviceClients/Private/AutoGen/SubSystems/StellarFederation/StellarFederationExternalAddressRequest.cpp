
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationExternalAddressRequest.h"

void UStellarFederationExternalAddressRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationExternalAddressRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/ExternalAddress");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationExternalAddressRequest::BuildBody(FString& BodyString) const
{
	
}

UStellarFederationExternalAddressRequest* UStellarFederationExternalAddressRequest::Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationExternalAddressRequest* Req = NewObject<UStellarFederationExternalAddressRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	

	return Req;
}
