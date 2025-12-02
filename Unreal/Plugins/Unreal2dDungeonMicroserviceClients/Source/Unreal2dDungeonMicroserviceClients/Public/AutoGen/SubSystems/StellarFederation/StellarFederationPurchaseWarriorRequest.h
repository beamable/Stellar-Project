
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/PurchaseWarriorRequestArgs.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#include "StellarFederationPurchaseWarriorRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationPurchaseWarriorRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="", Category="Beam")
	UPurchaseWarriorRequestArgs* Body = {};

	// Beam Base Request Declaration
	UStellarFederationPurchaseWarriorRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationPurchaseWarrior",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationPurchaseWarriorRequest* Make(FString _ListingId, UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationPurchaseWarriorSuccess, FBeamRequestContext, Context, UStellarFederationPurchaseWarriorRequest*, Request, UBeamPlainTextResponseBody*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationPurchaseWarriorError, FBeamRequestContext, Context, UStellarFederationPurchaseWarriorRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationPurchaseWarriorComplete, FBeamRequestContext, Context, UStellarFederationPurchaseWarriorRequest*, Request);

using FStellarFederationPurchaseWarriorFullResponse = FBeamFullResponse<UStellarFederationPurchaseWarriorRequest*, UBeamPlainTextResponseBody*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationPurchaseWarriorFullResponse, FStellarFederationPurchaseWarriorFullResponse);
