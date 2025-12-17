

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationCreateAccount.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationCreateAccountRequest.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/AccountResponse.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationCreateAccount"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, CreateAccount);
}

FName UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationCreateAccountRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetEndpointName() const
{
	return TEXT("CreateAccount");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetRequestClass() const
{
	return UStellarFederationCreateAccountRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetResponseClass() const
{
	return UAccountResponse::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationCreateAccountSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationCreateAccountError");
}

FString UK2BeamNode_ApiRequest_StellarFederationCreateAccount::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationCreateAccountComplete");
}

#undef LOCTEXT_NAMESPACE
