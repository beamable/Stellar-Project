
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/RemoveItemRequestArgs.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#include "StellarFederationRemoveItemRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationRemoveItemRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="", Category="Beam")
	URemoveItemRequestArgs* Body = {};

	// Beam Base Request Declaration
	UStellarFederationRemoveItemRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationRemoveItem",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationRemoveItemRequest* Make(FString _ItemContentId, int64 _InstanceId, UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationRemoveItemSuccess, FBeamRequestContext, Context, UStellarFederationRemoveItemRequest*, Request, UBeamPlainTextResponseBody*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationRemoveItemError, FBeamRequestContext, Context, UStellarFederationRemoveItemRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationRemoveItemComplete, FBeamRequestContext, Context, UStellarFederationRemoveItemRequest*, Request);

using FStellarFederationRemoveItemFullResponse = FBeamFullResponse<UStellarFederationRemoveItemRequest*, UBeamPlainTextResponseBody*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationRemoveItemFullResponse, FStellarFederationRemoveItemFullResponse);
