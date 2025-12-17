
#pragma once

#include "CoreMinimal.h"
#include "BeamBackend/BeamBaseRequestInterface.h"
#include "BeamBackend/BeamRequestContext.h"
#include "BeamBackend/BeamErrorResponse.h"
#include "BeamBackend/BeamFullResponse.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/JobsRequestArgs.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SchedulerJobResponse.h"

#include "StellarFederationJobsRequest.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEONMICROSERVICECLIENTS_API UStellarFederationJobsRequest : public UObject, public IBeamBaseRequestInterface
{
	GENERATED_BODY()
	
public:

	// Path Params
	
	
	// Query Params
	

	// Body Params
	UPROPERTY(EditAnywhere, BlueprintReadWrite, DisplayName="", Category="Beam")
	UJobsRequestArgs* Body = {};

	// Beam Base Request Declaration
	UStellarFederationJobsRequest() = default;

	virtual void BuildVerb(FString& VerbString) const override;
	virtual void BuildRoute(FString& RouteString) const override;
	virtual void BuildBody(FString& BodyString) const override;

	UFUNCTION(BlueprintPure, BlueprintInternalUseOnly, Category="Beam|StellarFederation|Utils|Make/Break", DisplayName="Make StellarFederationJobs",  meta=(DefaultToSelf="RequestOwner", AdvancedDisplay="RequestOwner", AutoCreateRefTerm="CustomHeaders"))
	static UStellarFederationJobsRequest* Make(bool _bEnable, UObject* RequestOwner, TMap<FString, FString> CustomHeaders);
};

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationJobsSuccess, FBeamRequestContext, Context, UStellarFederationJobsRequest*, Request, USchedulerJobResponse*, Response);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_ThreeParams(FOnStellarFederationJobsError, FBeamRequestContext, Context, UStellarFederationJobsRequest*, Request, FBeamErrorResponse, Error);

UDELEGATE(BlueprintAuthorityOnly)
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnStellarFederationJobsComplete, FBeamRequestContext, Context, UStellarFederationJobsRequest*, Request);

using FStellarFederationJobsFullResponse = FBeamFullResponse<UStellarFederationJobsRequest*, USchedulerJobResponse*>;
DECLARE_DELEGATE_OneParam(FOnStellarFederationJobsFullResponse, FStellarFederationJobsFullResponse);
