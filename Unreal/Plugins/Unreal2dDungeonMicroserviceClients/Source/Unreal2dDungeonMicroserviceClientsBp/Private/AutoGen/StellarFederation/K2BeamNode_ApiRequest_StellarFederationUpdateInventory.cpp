

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationUpdateInventory.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationUpdateInventoryRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationUpdateInventory"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, UpdateInventory);
}

FName UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationUpdateInventoryRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetEndpointName() const
{
	return TEXT("UpdateInventory");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetRequestClass() const
{
	return UStellarFederationUpdateInventoryRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationUpdateInventorySuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationUpdateInventoryError");
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateInventory::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationUpdateInventoryComplete");
}

#undef LOCTEXT_NAMESPACE
