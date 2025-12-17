// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Content/BeamContentTypes/BeamCurrencyContent.h"
#include "UObject/Object.h"
#include "GoldCoin.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEON_API UGoldCoin : public UBeamCurrencyContent
{
	GENERATED_BODY()

public:
	UFUNCTION()
	void GetContentType_GoldCoin(FString& Result){Result = TEXT("gold");}
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FName name;
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	FString symbol;
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	int32 decimals;
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	FString image;
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	FString description;
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	int64 totalSupply;
};
