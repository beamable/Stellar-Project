
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/DoOwnWarriorRequestArgs.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationDoOwnWarriorResponse.h"

#include "StellarFederationDoOwnWarriorRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationDoOwnWarriorRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="", Category="Beam")
	UDoOwnWarriorRequestArgs* Body = {};

	// Beam Base Request Declaration
	UStellarFederationDoOwnWarriorRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationDoOwnWarrior",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationDoOwnWarriorRequest* Make(FString _ItemContentId, UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationDoOwnWarriorSuccess, FBeamRequestContext, Context, UStellarFederationDoOwnWarriorRequest*, Request, UStellarFederationDoOwnWarriorResponse*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationDoOwnWarriorError, FBeamRequestContext, Context, UStellarFederationDoOwnWarriorRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationDoOwnWarriorComplete, FBeamRequestContext, Context, UStellarFederationDoOwnWarriorRequest*, Request);

using FStellarFederationDoOwnWarriorFullResponse = FBeamFullResponse<UStellarFederationDoOwnWarriorRequest*, UStellarFederationDoOwnWarriorResponse*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationDoOwnWarriorFullResponse, FStellarFederationDoOwnWarriorFullResponse);
