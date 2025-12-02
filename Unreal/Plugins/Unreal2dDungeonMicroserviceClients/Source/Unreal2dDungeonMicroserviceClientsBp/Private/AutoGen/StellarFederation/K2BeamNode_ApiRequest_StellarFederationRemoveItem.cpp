

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationRemoveItem.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationRemoveItemRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationRemoveItem"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, RemoveItem);
}

FName UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationRemoveItemRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetEndpointName() const
{
	return TEXT("RemoveItem");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetRequestClass() const
{
	return UStellarFederationRemoveItemRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationRemoveItemSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationRemoveItemError");
}

FString UK2BeamNode_ApiRequest_StellarFederationRemoveItem::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationRemoveItemComplete");
}

#undef LOCTEXT_NAMESPACE
