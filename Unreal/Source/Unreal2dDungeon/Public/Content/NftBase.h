// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/Interface.h"
#include "NftBase.generated.h"

UINTERFACE(BlueprintType)
class UNREAL2DDUNGEON_API UNftBase : public UInterface
{
	GENERATED_BODY()
};

/**
 * Unreal analogue of C# INftBase
 */
class UNREAL2DDUNGEON_API INftBase
{
	GENERATED_BODY()

	// Add interface functions to this class. This is the class that will be inherited to implement this interface.
public:
	UFUNCTION(BlueprintCallable, BlueprintNativeEvent, Category="Stellar")
	FString GetName() const;
	
	UFUNCTION(BlueprintCallable, BlueprintNativeEvent, Category="Stellar")
	FString GetDescription() const;
	
	UFUNCTION(BlueprintCallable, BlueprintNativeEvent, Category="Stellar")
	FString GetImage() const;
	
	UFUNCTION(BlueprintCallable, BlueprintNativeEvent, Category="Stellar")
	TMap<FString, FString> GetCustomProperties() const;
};
