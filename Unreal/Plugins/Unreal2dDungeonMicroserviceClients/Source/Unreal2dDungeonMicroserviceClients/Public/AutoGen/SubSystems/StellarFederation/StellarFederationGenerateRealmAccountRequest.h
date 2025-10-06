
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGenerateRealmAccountResponse.h"

#include "StellarFederationGenerateRealmAccountRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationGenerateRealmAccountRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	

	// Beam Base Request Declaration
	UStellarFederationGenerateRealmAccountRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationGenerateRealmAccount",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationGenerateRealmAccountRequest* Make(UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationGenerateRealmAccountSuccess, FBeamRequestContext, Context, UStellarFederationGenerateRealmAccountRequest*, Request, UStellarFederationGenerateRealmAccountResponse*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationGenerateRealmAccountError, FBeamRequestContext, Context, UStellarFederationGenerateRealmAccountRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationGenerateRealmAccountComplete, FBeamRequestContext, Context, UStellarFederationGenerateRealmAccountRequest*, Request);

using FStellarFederationGenerateRealmAccountFullResponse = FBeamFullResponse<UStellarFederationGenerateRealmAccountRequest*, UStellarFederationGenerateRealmAccountResponse*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationGenerateRealmAccountFullResponse, FStellarFederationGenerateRealmAccountFullResponse);
