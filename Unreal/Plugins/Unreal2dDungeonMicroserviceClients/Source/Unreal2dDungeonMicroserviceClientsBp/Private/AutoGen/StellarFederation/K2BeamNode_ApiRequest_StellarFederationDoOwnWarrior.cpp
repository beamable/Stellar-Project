

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationDoOwnWarrior.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationDoOwnWarriorRequest.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationDoOwnWarriorResponse.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationDoOwnWarrior"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, DoOwnWarrior);
}

FName UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationDoOwnWarriorRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetEndpointName() const
{
	return TEXT("DoOwnWarrior");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetRequestClass() const
{
	return UStellarFederationDoOwnWarriorRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetResponseClass() const
{
	return UStellarFederationDoOwnWarriorResponse::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationDoOwnWarriorSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationDoOwnWarriorError");
}

FString UK2BeamNode_ApiRequest_StellarFederationDoOwnWarrior::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationDoOwnWarriorComplete");
}

#undef LOCTEXT_NAMESPACE
