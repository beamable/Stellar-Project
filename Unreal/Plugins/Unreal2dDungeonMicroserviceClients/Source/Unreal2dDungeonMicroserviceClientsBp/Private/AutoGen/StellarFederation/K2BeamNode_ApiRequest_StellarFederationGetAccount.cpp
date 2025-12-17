

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationGetAccount.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationGetAccountRequest.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AccountResponse.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationGetAccount"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetAccount);
}

FName UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationGetAccountRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetEndpointName() const
{
	return TEXT("GetAccount");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetRequestClass() const
{
	return UStellarFederationGetAccountRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetResponseClass() const
{
	return UAccountResponse::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationGetAccountSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationGetAccountError");
}

FString UK2BeamNode_ApiRequest_StellarFederationGetAccount::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationGetAccountComplete");
}

#undef LOCTEXT_NAMESPACE
