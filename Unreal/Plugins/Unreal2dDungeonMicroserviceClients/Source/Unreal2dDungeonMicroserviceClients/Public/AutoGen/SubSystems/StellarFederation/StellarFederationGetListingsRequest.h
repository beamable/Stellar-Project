
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/GetListingsRequestArgs.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/GetListingsResponse.h"

#include "StellarFederationGetListingsRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationGetListingsRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="", Category="Beam")
	UGetListingsRequestArgs* Body = {};

	// Beam Base Request Declaration
	UStellarFederationGetListingsRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationGetListings",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationGetListingsRequest* Make(FString _StoreId, UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationGetListingsSuccess, FBeamRequestContext, Context, UStellarFederationGetListingsRequest*, Request, UGetListingsResponse*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationGetListingsError, FBeamRequestContext, Context, UStellarFederationGetListingsRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationGetListingsComplete, FBeamRequestContext, Context, UStellarFederationGetListingsRequest*, Request);

using FStellarFederationGetListingsFullResponse = FBeamFullResponse<UStellarFederationGetListingsRequest*, UGetListingsResponse*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationGetListingsFullResponse, FStellarFederationGetListingsFullResponse);
