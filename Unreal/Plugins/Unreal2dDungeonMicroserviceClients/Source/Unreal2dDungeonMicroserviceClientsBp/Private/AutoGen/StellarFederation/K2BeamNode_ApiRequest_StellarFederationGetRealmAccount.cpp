

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationGetRealmAccount.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationGetRealmAccountRequest.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationGetRealmAccountResponse.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationGetRealmAccount"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetRealmAccount);
}

FName UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationGetRealmAccountRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetEndpointName() const
{
	return TEXT("GetRealmAccount");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetRequestClass() const
{
	return UStellarFederationGetRealmAccountRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetResponseClass() const
{
	return UStellarFederationGetRealmAccountResponse::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationGetRealmAccountSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationGetRealmAccountError");
}

FString UK2BeamNode_ApiRequest_StellarFederationGetRealmAccount::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationGetRealmAccountComplete");
}

#undef LOCTEXT_NAMESPACE
