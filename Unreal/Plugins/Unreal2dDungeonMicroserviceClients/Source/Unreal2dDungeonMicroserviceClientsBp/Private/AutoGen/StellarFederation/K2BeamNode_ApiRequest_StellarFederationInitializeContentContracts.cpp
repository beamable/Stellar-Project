

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationInitializeContentContracts.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationInitializeContentContractsRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationInitializeContentContracts"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, InitializeContentContracts);
}

FName UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationInitializeContentContractsRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetEndpointName() const
{
	return TEXT("InitializeContentContracts");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetRequestClass() const
{
	return UStellarFederationInitializeContentContractsRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationInitializeContentContractsSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationInitializeContentContractsError");
}

FString UK2BeamNode_ApiRequest_StellarFederationInitializeContentContracts::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationInitializeContentContractsComplete");
}

#undef LOCTEXT_NAMESPACE
