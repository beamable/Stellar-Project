

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationBlockProcessor.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationBlockProcessorRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationBlockProcessor"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, BlockProcessor);
}

FName UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationBlockProcessorRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetEndpointName() const
{
	return TEXT("BlockProcessor");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetRequestClass() const
{
	return UStellarFederationBlockProcessorRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationBlockProcessorSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationBlockProcessorError");
}

FString UK2BeamNode_ApiRequest_StellarFederationBlockProcessor::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationBlockProcessorComplete");
}

#undef LOCTEXT_NAMESPACE
