

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationExternalSignature.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationExternalSignatureRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationExternalSignature"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, ExternalSignature);
}

FName UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationExternalSignatureRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetEndpointName() const
{
	return TEXT("ExternalSignature");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetRequestClass() const
{
	return UStellarFederationExternalSignatureRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationExternalSignatureSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationExternalSignatureError");
}

FString UK2BeamNode_ApiRequest_StellarFederationExternalSignature::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationExternalSignatureComplete");
}

#undef LOCTEXT_NAMESPACE
