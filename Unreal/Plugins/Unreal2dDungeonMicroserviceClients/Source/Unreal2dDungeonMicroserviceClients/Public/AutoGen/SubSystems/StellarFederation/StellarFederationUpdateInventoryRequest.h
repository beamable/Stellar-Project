
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/UpdateInventoryRequestArgs.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#include "StellarFederationUpdateInventoryRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationUpdateInventoryRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="", Category="Beam")
	UUpdateInventoryRequestArgs* Body = {};

	// Beam Base Request Declaration
	UStellarFederationUpdateInventoryRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationUpdateInventory",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationUpdateInventoryRequest* Make(FString _CurrencyContentId, int32 _Amount, TArray<UCropUpdateRequestBody*> _Items, UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationUpdateInventorySuccess, FBeamRequestContext, Context, UStellarFederationUpdateInventoryRequest*, Request, UBeamPlainTextResponseBody*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationUpdateInventoryError, FBeamRequestContext, Context, UStellarFederationUpdateInventoryRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationUpdateInventoryComplete, FBeamRequestContext, Context, UStellarFederationUpdateInventoryRequest*, Request);

using FStellarFederationUpdateInventoryFullResponse = FBeamFullResponse<UStellarFederationUpdateInventoryRequest*, UBeamPlainTextResponseBody*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationUpdateInventoryFullResponse, FStellarFederationUpdateInventoryFullResponse);
