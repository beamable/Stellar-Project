

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationUpdateItems.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationUpdateItemsRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationUpdateItems"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, UpdateItems);
}

FName UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationUpdateItemsRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetEndpointName() const
{
	return TEXT("UpdateItems");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetRequestClass() const
{
	return UStellarFederationUpdateItemsRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationUpdateItemsSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationUpdateItemsError");
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateItems::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationUpdateItemsComplete");
}

#undef LOCTEXT_NAMESPACE
