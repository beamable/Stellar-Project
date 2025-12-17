
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationExternalSignatureRequest.h"

void UStellarFederationExternalSignatureRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationExternalSignatureRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/ExternalSignature");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationExternalSignatureRequest::BuildBody(FString& BodyString) const
{
	
}

UStellarFederationExternalSignatureRequest* UStellarFederationExternalSignatureRequest::Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationExternalSignatureRequest* Req = NewObject<UStellarFederationExternalSignatureRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	

	return Req;
}
