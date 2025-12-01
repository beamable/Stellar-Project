
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateCurrencyRequestArgs.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#include "StellarFederationUpdateCurrencyRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationUpdateCurrencyRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="", Category="Beam")
	UUpdateCurrencyRequestArgs* Body = {};

	// Beam Base Request Declaration
	UStellarFederationUpdateCurrencyRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationUpdateCurrency",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationUpdateCurrencyRequest* Make(FString _CurrencyContentId, int32 _Amount, UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationUpdateCurrencySuccess, FBeamRequestContext, Context, UStellarFederationUpdateCurrencyRequest*, Request, UBeamPlainTextResponseBody*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationUpdateCurrencyError, FBeamRequestContext, Context, UStellarFederationUpdateCurrencyRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationUpdateCurrencyComplete, FBeamRequestContext, Context, UStellarFederationUpdateCurrencyRequest*, Request);

using FStellarFederationUpdateCurrencyFullResponse = FBeamFullResponse<UStellarFederationUpdateCurrencyRequest*, UBeamPlainTextResponseBody*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationUpdateCurrencyFullResponse, FStellarFederationUpdateCurrencyFullResponse);
