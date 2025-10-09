// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "StellarController.generated.h"

/**
 * 
 */
UCLASS(BlueprintType, Blueprintable)
class UNREAL2DDUNGEON_API UStellarController : public UObject
{
	GENERATED_BODY()

public:
	/**
	 * This will be useful to get the identity info
	 * for backend calls
	 */
	UFUNCTION(BlueprintPure, Category = "Stellar")
	void GetStellarSettings(FString& MicroserviceId, FString& FederationId,
		FString& StellarExternalId, FString& AddressChannel, FString& SignatureChannel)
	{
		MicroserviceId = TEXT("StellarFederation");
		FederationId = TEXT("StellarIdentity");
		StellarExternalId = TEXT("SuiExternalIdentity");
		AddressChannel = TEXT("external-auth-address");
		SignatureChannel = TEXT("external-auth-signature");
	}
};
