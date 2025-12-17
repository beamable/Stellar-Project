

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationExternalAddress.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationExternalAddressRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationExternalAddress"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, ExternalAddress);
}

FName UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationExternalAddressRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetEndpointName() const
{
	return TEXT("ExternalAddress");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetRequestClass() const
{
	return UStellarFederationExternalAddressRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationExternalAddressSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationExternalAddressError");
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalAddress::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationExternalAddressComplete");
}

#undef LOCTEXT_NAMESPACE
