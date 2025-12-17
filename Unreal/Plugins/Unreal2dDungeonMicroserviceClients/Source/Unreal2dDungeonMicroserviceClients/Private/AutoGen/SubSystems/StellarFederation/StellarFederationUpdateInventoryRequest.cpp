
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationUpdateInventoryRequest.h"

void UStellarFederationUpdateInventoryRequest::BuildVerb(FString& VerbString) const
{
	VerbString = TEXT("POST");
}

void UStellarFederationUpdateInventoryRequest::BuildRoute(FString& RouteString) const
{
	FString Route = TEXT("micro_StellarFederation/UpdateInventory");
	
	
	FString QueryParams = TEXT("");
	QueryParams.Reserve(1024);
	bool bIsFirstQueryParam = true;
	
	RouteString.Appendf(TEXT("%s%s"), *Route, *QueryParams);		
}

void UStellarFederationUpdateInventoryRequest::BuildBody(FString& BodyString) const
{
	ensureAlways(Body);

	TUnrealJsonSerializer JsonSerializer = TJsonStringWriter<TCondensedJsonPrintPolicy<TCHAR>>::Create(&BodyString);
	Body->BeamSerialize(JsonSerializer);
	JsonSerializer->Close();
}

UStellarFederationUpdateInventoryRequest* UStellarFederationUpdateInventoryRequest::Make(FString _CurrencyContentId, int32 _Amount, TArray<UCropUpdateRequestBody*> _Items, UObject* RequestOwner, TMap<FString, FString> CustomHeaders)
{
	UStellarFederationUpdateInventoryRequest* Req = NewObject<UStellarFederationUpdateInventoryRequest>(RequestOwner);
	Req->CustomHeaders = TMap{CustomHeaders};

	// Pass in Path and Query Parameters (Blank if no path parameters exist)
	
	
	// Makes a body and fill up with parameters (Blank if no body parameters exist)
	Req->Body = NewObject<UUpdateInventoryRequestArgs>(Req);
	Req->Body->CurrencyContentId = _CurrencyContentId;
	Req->Body->Amount = _Amount;
	Req->Body->Items = _Items;
	

	return Req;
}
