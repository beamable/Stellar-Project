

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationJobs.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationJobsRequest.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SchedulerJobResponse.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationJobs"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationJobs::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationJobs::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, Jobs);
}

FName UK2BeamNode_ApiRequest_StellarFederationJobs::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationJobsRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationJobs::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationJobs::GetEndpointName() const
{
	return TEXT("Jobs");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationJobs::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationJobs::GetRequestClass() const
{
	return UStellarFederationJobsRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationJobs::GetResponseClass() const
{
	return USchedulerJobResponse::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationJobs::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationJobsSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationJobs::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationJobsError");
}

FString UK2BeamNode_ApiRequest_StellarFederationJobs::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationJobsComplete");
}

#undef LOCTEXT_NAMESPACE
