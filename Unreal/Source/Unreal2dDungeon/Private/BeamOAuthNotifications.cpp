#include "Unreal2dDungeon/Public/BeamOAuthNotifications.h"

#include "Engine/Engine.h"
#include "Engine/GameInstance.h"
#include "Runtime/BeamRuntime.h"

// Helper
UBeamOAuthNotifications* UBeamOAuthNotifications::Get(UObject* WorldContextObject)
{
	if (!WorldContextObject) return nullptr;

	if (UWorld* World = GEngine->GetWorldFromContextObject(WorldContextObject, EGetWorldErrorMode::LogAndReturnNull))
	{
		if (UGameInstance* GI = World->GetGameInstance())
		{
			return GI->GetSubsystem<UBeamOAuthNotifications>();
		}
	}
	UE_LOG(LogTemp, Warning, TEXT("UBeamOAuthNotifications::Get failed (no valid world/game instance)."));
	return nullptr;
}

int64 UBeamOAuthNotifications::SubscribeToNotification(FUserSlot Slot, const FString& Key, UObject* ContextObject)
{
	if (UBeamRuntime* Runtime = UBeamRuntime::GetSelf(ContextObject))
	{
		// Forward runtime messages into our Blueprint multicast.
		FOnCustomNotification LocalHandler;
		TWeakObjectPtr<UBeamOAuthNotifications> WeakThis(this);
		LocalHandler.BindLambda([WeakThis](const FOAuthNotificationMessage& Msg)
		{
			if (WeakThis.IsValid())
			{
				WeakThis->OnOAuthNotification.Broadcast(Msg);
			}
		});

		const FDelegateHandle Handle =
			Runtime->SubscribeToCustomNotification<FOnCustomNotification, FOAuthNotificationMessage>(Slot, Key, LocalHandler);

		if (Handle.IsValid())
		{
			const int64 Token = NextToken++;
			Bindings.Add(Token, FNotifBinding{Slot, Key, Handle});
			return Token;
		}
	}

	UE_LOG(LogTemp, Warning, TEXT("SubscribeToNotification failed for Key=%s"), *Key);
	return 0;
}

bool UBeamOAuthNotifications::UnsubscribeByToken(const int64 Token, UObject* ContextObject)
{
	FNotifBinding Binding;
	if (!Bindings.RemoveAndCopyValue(Token, Binding))
	{
		UE_LOG(LogTemp, Warning, TEXT("UnsubscribeByToken: token %lld not found."), Token);
		return false;
	}

	if (UBeamRuntime* Runtime = UBeamRuntime::GetSelf(ContextObject))
	{
		const bool bOk = Runtime->UnsubscribeToCustomNotification(Binding.Slot, Binding.Key, Binding.Handle);
		if (!bOk)
		{
			UE_LOG(LogTemp, Warning, TEXT("UnsubscribeToCustomNotification failed (Key=%s)."), *Binding.Key);
		}
		return bOk;
	}

	UE_LOG(LogTemp, Warning, TEXT("UnsubscribeByToken: missing runtime."));
	return false;
}
