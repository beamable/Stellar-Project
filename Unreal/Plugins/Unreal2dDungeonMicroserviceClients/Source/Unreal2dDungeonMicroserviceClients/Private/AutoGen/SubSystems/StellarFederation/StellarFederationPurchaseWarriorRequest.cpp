
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationPurchaseWarriorRequest.h"

void UStellarFederationPurchaseWarriorRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationPurchaseWarriorRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/PurchaseWarrior");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationPurchaseWarriorRequest::BuildBody(FString& BodyString) const
{
	ensureAlways(Body);

	TUnrealJsonSerializer JsonSerializer = TJsonStringWriter<TCondensedJsonPrintPolicy<TCHAR>>::Create(&BodyString);
	Body->BeamSerialize(JsonSerializer);
	JsonSerializer->Close();
}

UStellarFederationPurchaseWarriorRequest* UStellarFederationPurchaseWarriorRequest::Make(FString _ListingId, UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationPurchaseWarriorRequest* Req = NewObject<UStellarFederationPurchaseWarriorRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	Req->Body = NewObject<UPurchaseWarriorRequestArgs>(Req);
	Req->Body->ListingId = _ListingId;
	

	return Req;
}
