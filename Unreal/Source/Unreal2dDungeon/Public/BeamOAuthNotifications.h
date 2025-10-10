#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "UserSlots/UserSlot.h"
#include "BeamOAuthNotifications.generated.h"

/**
 * JSON payload *inside* messageFull.
 * Add more strongly-typed fields as your payload evolves.
 */
USTRUCT(BlueprintType)
struct UNREAL2DDUNGEON_API FOAuthNotificationMessage : public FBeamJsonSerializableUStruct
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadOnly, Category="Beam|Notifications") FString Status;
	UPROPERTY(BlueprintReadOnly, Category="Beam|Notifications") FString Code;
	UPROPERTY(BlueprintReadOnly, Category="Beam|Notifications") FString Error;

	/** Catch-all for extra string fields. */
	UPROPERTY(BlueprintReadOnly, Category="Beam|Notifications") TMap<FString, FString> Fields;

	// Client only deserializes.
	virtual void BeamSerializeProperties(TUnrealJsonSerializer&) const override {}
	virtual void BeamSerializeProperties(TUnrealPrettyJsonSerializer&) const override {}

	/** Bag == parsed JSON of messageFull. */
	virtual void BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag) override
	{
		auto GetStr = [&](const TCHAR* K, FString& Out){ if (Bag->HasField(K)) Out = Bag->GetStringField(K); };
		GetStr(TEXT("status"), Status);
		GetStr(TEXT("code"),   Code);
		GetStr(TEXT("error"),  Error);

		Fields.Empty();
		for (const auto& Pair : Bag->Values)
		{
			if (Pair.Value.IsValid() && Pair.Value->Type == EJson::String)
			{
				Fields.Add(Pair.Key, Pair.Value->AsString());
			}
		}
	}
};

/** Plain C++ delegate used only internally to bind to Beam runtime. */
DECLARE_DELEGATE_OneParam(FOnCustomNotification, const FOAuthNotificationMessage&);

/** Record we keep so users can unsubscribe a single callback by token. */
USTRUCT()
struct FNotifBinding
{
	GENERATED_BODY()
	FUserSlot       Slot;
	FString         Key;
	FDelegateHandle Handle;
	FOnCustomNotification Delegate;

};

/** Blueprint multicast users actually subscribe to. */
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOAuthNotificationBP, const FOAuthNotificationMessage&, Message);

/**
 * Subsystem that manages Beam custom notifications:
 * - Internally subscribes to Beam runtime and tracks FDelegateHandle(s)
 * - Exposes a Blueprint multicast for gameplay/UI
 * - Provides unsubscribe-by-token API
 */
UCLASS(BlueprintType)
class UNREAL2DDUNGEON_API UBeamOAuthNotifications : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	/** Subscribe to a Slot+Key stream. Returns a token for UnsubscribeByToken (0 on failure). */
	UFUNCTION(BlueprintCallable, Category="Beam|Notifications", meta=(DefaultToSelf="ContextObject"))
	int64 SubscribeToNotification(FUserSlot Slot, const FString& Key, UObject* ContextObject);

	/** Unsubscribe a single binding by token. */
	UFUNCTION(BlueprintCallable, Category="Beam|Notifications", meta=(DefaultToSelf="ContextObject"))
	bool UnsubscribeByToken(int64 Token, UObject* ContextObject);

	/** Blueprint event listeners bind here (Add/Remove). */
	UPROPERTY(BlueprintAssignable, Category="Beam|Notifications")
	FOAuthNotificationBP OnOAuthNotification;

	/** Convenience getter. */
	UFUNCTION(BlueprintPure, Category="Beam|Notifications",
		meta=(WorldContext="WorldContextObject", DisplayName="Get OAuth Notifications Subsystem"))
	static UBeamOAuthNotifications* Get(UObject* WorldContextObject);

	UFUNCTION(BlueprintCallable, Category="Beam|Notifications", meta=(DefaultToSelf="ContextObject"))
	int32 UnsubscribeAll(UObject* ContextObject)
	{
		int32 Count = 0;
		TArray<int64> Tokens;
		Bindings.GetKeys(Tokens);

		for (const int64 Token : Tokens)
		{
			if (UnsubscribeByToken(Token, ContextObject))
			{
				++Count;
			}
		}
		return Count;
	}

private:
	/** Monotonic token generator and storage for per-subscription handles. */
	int64 NextToken = 1;
	TMap<int64, FNotifBinding> Bindings; // Token -> binding
};
