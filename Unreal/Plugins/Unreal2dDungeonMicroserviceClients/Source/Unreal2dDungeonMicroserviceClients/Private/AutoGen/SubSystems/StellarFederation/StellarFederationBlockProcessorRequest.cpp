
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationBlockProcessorRequest.h"

void UStellarFederationBlockProcessorRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationBlockProcessorRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/BlockProcessor");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationBlockProcessorRequest::BuildBody(FString& BodyString) const
{
	
}

UStellarFederationBlockProcessorRequest* UStellarFederationBlockProcessorRequest::Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationBlockProcessorRequest* Req = NewObject<UStellarFederationBlockProcessorRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	

	return Req;
}
