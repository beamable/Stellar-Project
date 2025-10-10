// Fill out your copyright notice in the Description page of Project Settings.


#include "StellarController.h"
#include "GenericPlatform/GenericPlatformHttp.h"  // for FGenericPlatformHttp

FString UStellarController::UrlEncode(const FString& Unencoded)
{
	return FGenericPlatformHttp::UrlEncode(Unencoded);
}
