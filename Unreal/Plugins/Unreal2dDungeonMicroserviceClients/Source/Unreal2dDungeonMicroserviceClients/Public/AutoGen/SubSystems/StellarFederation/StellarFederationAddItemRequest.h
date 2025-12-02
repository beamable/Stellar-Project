
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AddItemRequestArgs.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationAddItemResponse.h"

#include "StellarFederationAddItemRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationAddItemRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="", Category="Beam")
	UAddItemRequestArgs* Body = {};

	// Beam Base Request Declaration
	UStellarFederationAddItemRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationAddItem",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationAddItemRequest* Make(FString _ItemContentId, TMap<FString, FString> _Properties, UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationAddItemSuccess, FBeamRequestContext, Context, UStellarFederationAddItemRequest*, Request, UStellarFederationAddItemResponse*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationAddItemError, FBeamRequestContext, Context, UStellarFederationAddItemRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationAddItemComplete, FBeamRequestContext, Context, UStellarFederationAddItemRequest*, Request);

using FStellarFederationAddItemFullResponse = FBeamFullResponse<UStellarFederationAddItemRequest*, UStellarFederationAddItemResponse*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationAddItemFullResponse, FStellarFederationAddItemFullResponse);
