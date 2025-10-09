

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationStellarConfiguration.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationStellarConfigurationRequest.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ConfigurationResponse.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationStellarConfiguration"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, StellarConfiguration);
}

FName UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationStellarConfigurationRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetEndpointName() const
{
	return TEXT("StellarConfiguration");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetRequestClass() const
{
	return UStellarFederationStellarConfigurationRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetResponseClass() const
{
	return UConfigurationResponse::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationStellarConfigurationSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationStellarConfigurationError");
}

FString UK2BeamNode_ApiRequest_StellarFederationStellarConfiguration::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationStellarConfigurationComplete");
}

#undef LOCTEXT_NAMESPACE
