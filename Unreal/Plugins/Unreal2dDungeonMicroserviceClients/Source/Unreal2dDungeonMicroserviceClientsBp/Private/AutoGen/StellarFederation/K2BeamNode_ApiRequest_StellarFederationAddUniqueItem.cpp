

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationAddUniqueItem.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationAddUniqueItemRequest.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/StellarFederationAddUniqueItemResponse.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationAddUniqueItem"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, AddUniqueItem);
}

FName UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationAddUniqueItemRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetEndpointName() const
{
	return TEXT("AddUniqueItem");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetRequestClass() const
{
	return UStellarFederationAddUniqueItemRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetResponseClass() const
{
	return UStellarFederationAddUniqueItemResponse::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationAddUniqueItemSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationAddUniqueItemError");
}

FString UK2BeamNode_ApiRequest_StellarFederationAddUniqueItem::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationAddUniqueItemComplete");
}

#undef LOCTEXT_NAMESPACE
