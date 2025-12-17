
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AddUniqueItemRequestArgs.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationAddUniqueItemResponse.h"

#include "StellarFederationAddUniqueItemRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationAddUniqueItemRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="", Category="Beam")
	UAddUniqueItemRequestArgs* Body = {};

	// Beam Base Request Declaration
	UStellarFederationAddUniqueItemRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationAddUniqueItem",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationAddUniqueItemRequest* Make(FString _ItemContentId, TMap<FString, FString> _Properties, UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationAddUniqueItemSuccess, FBeamRequestContext, Context, UStellarFederationAddUniqueItemRequest*, Request, UStellarFederationAddUniqueItemResponse*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationAddUniqueItemError, FBeamRequestContext, Context, UStellarFederationAddUniqueItemRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationAddUniqueItemComplete, FBeamRequestContext, Context, UStellarFederationAddUniqueItemRequest*, Request);

using FStellarFederationAddUniqueItemFullResponse = FBeamFullResponse<UStellarFederationAddUniqueItemRequest*, UStellarFederationAddUniqueItemResponse*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationAddUniqueItemFullResponse, FStellarFederationAddUniqueItemFullResponse);
