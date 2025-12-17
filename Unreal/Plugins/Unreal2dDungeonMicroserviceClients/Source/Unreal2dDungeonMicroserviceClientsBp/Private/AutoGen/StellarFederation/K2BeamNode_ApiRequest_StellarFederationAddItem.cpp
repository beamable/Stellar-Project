

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationAddItem.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationAddItemRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationAddItem"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationAddItem::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationAddItem::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, AddItem);
}

FName UK2BeamNode_ApiRequest_StellarFederationAddItem::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationAddItemRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationAddItem::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationAddItem::GetEndpointName() const
{
	return TEXT("AddItem");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationAddItem::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationAddItem::GetRequestClass() const
{
	return UStellarFederationAddItemRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationAddItem::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationAddItem::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationAddItemSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationAddItem::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationAddItemError");
}

FString UK2BeamNode_ApiRequest_StellarFederationAddItem::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationAddItemComplete");
}

#undef LOCTEXT_NAMESPACE
