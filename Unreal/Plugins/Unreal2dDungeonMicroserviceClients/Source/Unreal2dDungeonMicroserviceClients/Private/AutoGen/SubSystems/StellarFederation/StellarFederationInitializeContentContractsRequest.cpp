
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationInitializeContentContractsRequest.h"

void UStellarFederationInitializeContentContractsRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationInitializeContentContractsRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/InitializeContentContracts");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationInitializeContentContractsRequest::BuildBody(FString& BodyString) const
{
	
}

UStellarFederationInitializeContentContractsRequest* UStellarFederationInitializeContentContractsRequest::Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationInitializeContentContractsRequest* Req = NewObject<UStellarFederationInitializeContentContractsRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	

	return Req;
}
