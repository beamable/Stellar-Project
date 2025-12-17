
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Serialization/BeamPlainTextResponseBody.h"

#include "StellarFederationExternalAddressRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationExternalAddressRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	

	// Beam Base Request Declaration
	UStellarFederationExternalAddressRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationExternalAddress",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationExternalAddressRequest* Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationExternalAddressSuccess, FBeamRequestContext, Context, UStellarFederationExternalAddressRequest*, Request, UBeamPlainTextResponseBody*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationExternalAddressError, FBeamRequestContext, Context, UStellarFederationExternalAddressRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationExternalAddressComplete, FBeamRequestContext, Context, UStellarFederationExternalAddressRequest*, Request);

using FStellarFederationExternalAddressFullResponse = FBeamFullResponse<UStellarFederationExternalAddressRequest*, UBeamPlainTextResponseBody*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationExternalAddressFullResponse, FStellarFederationExternalAddressFullResponse);
