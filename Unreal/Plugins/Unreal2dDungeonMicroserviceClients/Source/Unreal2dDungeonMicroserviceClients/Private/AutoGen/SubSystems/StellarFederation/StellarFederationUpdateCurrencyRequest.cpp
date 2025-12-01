
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationUpdateCurrencyRequest.h"

void UStellarFederationUpdateCurrencyRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationUpdateCurrencyRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/UpdateCurrency");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationUpdateCurrencyRequest::BuildBody(FString& BodyString) const
{
	ensureAlways(Body);

	TUnrealJsonSerializer JsonSerializer = TJsonStringWriter<TCondensedJsonPrintPolicy<TCHAR>>::Create(&BodyString);
	Body->BeamSerialize(JsonSerializer);
	JsonSerializer->Close();
}

UStellarFederationUpdateCurrencyRequest* UStellarFederationUpdateCurrencyRequest::Make(FString _CurrencyContentId, int32 _Amount, UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationUpdateCurrencyRequest* Req = NewObject<UStellarFederationUpdateCurrencyRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	Req->Body = NewObject<UUpdateCurrencyRequestArgs>(Req);
	Req->Body->CurrencyContentId = _CurrencyContentId;
	Req->Body->Amount = _Amount;
	

	return Req;
}
