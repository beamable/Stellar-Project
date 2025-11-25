// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "NftBase.h"
#include "Content/BeamContentTypes/BeamItemContent.h"
#include "UObject/Object.h"
#include "WarriorItem.generated.h"

UCLASS(BlueprintType)
class UNREAL2DDUNGEON_API UWarriorItem : public UBeamItemContent , public INftBase
{
	GENERATED_BODY()
	
public:
	UFUNCTION()
	void GetContentType_WarriorItem(FString& Result){Result = TEXT("warrior");}
	
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	FName name;
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	FString description;
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	FString image;
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	TMap<FString, FString> customProperties;

	virtual FString GetName_Implementation() const { return name.ToString(); }
	virtual FString GetDescription_Implementation() const { return description; }
	virtual FString GetImage_Implementation() const { return image; }
	virtual TMap<FString,FString> GetCustomProperties_Implementation() const { return customProperties; }
};
