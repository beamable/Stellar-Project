

#include "Unreal2dDungeonMicroserviceClientsBp/Public/AutoGen/StellarFederation/K2BeamNode_ApiRequest_StellarFederationUpdateCurrency.h"

#include "BeamK2.h"

#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/BeamStellarFederationApi.h"
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/SubSystems/StellarFederation/StellarFederationUpdateCurrencyRequest.h"
#include "Serialization/BeamPlainTextResponseBody.h"

#define LOCTEXT_NAMESPACE "K2BeamNode_ApiRequest_StellarFederationUpdateCurrency"

using namespace BeamK2;

FName UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetSelfFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, GetSelf);
}

FName UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetRequestFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UBeamStellarFederationApi, UpdateCurrency);
}

FName UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetMakeFunctionName() const
{
	return GET_FUNCTION_NAME_CHECKED(UStellarFederationUpdateCurrencyRequest, Make);
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetServiceName() const
{
	return TEXT("StellarFederation");
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetEndpointName() const
{
	return TEXT("UpdateCurrency");
}

UClass* UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetApiClass() const
{
	return UBeamStellarFederationApi::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetRequestClass() const
{
	return UStellarFederationUpdateCurrencyRequest::StaticClass();
}

UClass* UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetResponseClass() const
{
	return UBeamPlainTextResponseBody::StaticClass();
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetRequestSuccessDelegateName() const
{
	return TEXT("OnStellarFederationUpdateCurrencySuccess");
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetRequestErrorDelegateName() const
{
	return TEXT("OnStellarFederationUpdateCurrencyError");
}

FString UK2BeamNode_ApiRequest_StellarFederationUpdateCurrency::GetRequestCompleteDelegateName() const
{
	return TEXT("OnStellarFederationUpdateCurrencyComplete");
}

#undef LOCTEXT_NAMESPACE
