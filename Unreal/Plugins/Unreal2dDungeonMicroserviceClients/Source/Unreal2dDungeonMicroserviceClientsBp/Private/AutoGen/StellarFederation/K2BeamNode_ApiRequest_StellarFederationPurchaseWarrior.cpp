

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationPurchaseWarrior.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationPurchaseWarriorRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationPurchaseWarrior"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, PurchaseWarrior);
}

FName UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationPurchaseWarriorRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetEndpointName() const
{
	return TEXT("PurchaseWarrior");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetRequestClass() const
{
	return UStellarFederationPurchaseWarriorRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationPurchaseWarriorSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationPurchaseWarriorError");
}

FString UK2BeamNode_ApiRequest_StellarFederationPurchaseWarrior::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationPurchaseWarriorComplete");
}

#undef LOCTEXT_NAMESPACE
