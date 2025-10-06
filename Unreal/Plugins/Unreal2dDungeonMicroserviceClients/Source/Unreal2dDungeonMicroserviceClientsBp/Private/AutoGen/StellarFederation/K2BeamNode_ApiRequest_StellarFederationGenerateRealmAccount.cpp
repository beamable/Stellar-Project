

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationGenerateRealmAccountRequest.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGenerateRealmAccountResponse.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GenerateRealmAccount);
}

FName UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationGenerateRealmAccountRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetEndpointName() const
{
	return TEXT("GenerateRealmAccount");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetRequestClass() const
{
	return UStellarFederationGenerateRealmAccountRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetResponseClass() const
{
	return UStellarFederationGenerateRealmAccountResponse::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationGenerateRealmAccountSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationGenerateRealmAccountError");
}

FString UK2BeamNode_ApiRequest_StellarFederationGenerateRealmAccount::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationGenerateRealmAccountComplete");
}

#undef LOCTEXT_NAMESPACE
