
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGetRealmAccountResponse.h"

#include "StellarFederationGetRealmAccountRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationGetRealmAccountRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	

	// Beam Base Request Declaration
	UStellarFederationGetRealmAccountRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationGetRealmAccount",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationGetRealmAccountRequest* Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationGetRealmAccountSuccess, FBeamRequestContext, Context, UStellarFederationGetRealmAccountRequest*, Request, UStellarFederationGetRealmAccountResponse*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationGetRealmAccountError, FBeamRequestContext, Context, UStellarFederationGetRealmAccountRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationGetRealmAccountComplete, FBeamRequestContext, Context, UStellarFederationGetRealmAccountRequest*, Request);

using FStellarFederationGetRealmAccountFullResponse = FBeamFullResponse<UStellarFederationGetRealmAccountRequest*, UStellarFederationGetRealmAccountResponse*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationGetRealmAccountFullResponse, FStellarFederationGetRealmAccountFullResponse);
