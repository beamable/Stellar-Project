
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Serialization/BeamPlainTextResponseBody.h"

#include "StellarFederationBlockProcessorRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationBlockProcessorRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	

	// Beam Base Request Declaration
	UStellarFederationBlockProcessorRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationBlockProcessor",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationBlockProcessorRequest* Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationBlockProcessorSuccess, FBeamRequestContext, Context, UStellarFederationBlockProcessorRequest*, Request, UBeamPlainTextResponseBody*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationBlockProcessorError, FBeamRequestContext, Context, UStellarFederationBlockProcessorRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationBlockProcessorComplete, FBeamRequestContext, Context, UStellarFederationBlockProcessorRequest*, Request);

using FStellarFederationBlockProcessorFullResponse = FBeamFullResponse<UStellarFederationBlockProcessorRequest*, UBeamPlainTextResponseBody*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationBlockProcessorFullResponse, FStellarFederationBlockProcessorFullResponse);
