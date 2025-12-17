

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationGetListings.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationGetListingsRequest.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/GetListingsResponse.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationGetListings"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationGetListings::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationGetListings::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetListings);
}

FName UK2BeamNode_ApiRequest_StellarFederationGetListings::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationGetListingsRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationGetListings::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationGetListings::GetEndpointName() const
{
	return TEXT("GetListings");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGetListings::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGetListings::GetRequestClass() const
{
	return UStellarFederationGetListingsRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationGetListings::GetResponseClass() const
{
	return UGetListingsResponse::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationGetListings::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationGetListingsSuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationGetListings::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationGetListingsError");
}

FString UK2BeamNode_ApiRequest_StellarFederationGetListings::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationGetListingsComplete");
}

#undef LOCTEXT_NAMESPACE
