import { useCallback, useEffect, useReducer, useRef, useState } from "react"

const WALLET_WINDOW_NAME = "stellarWalletBridge"
const WALLET_WINDOW_FEATURES =
  "noopener,noreferrer,width=480,height=780,resizable=yes,scrollbars=yes,menubar=no,toolbar=no"

type WalletWindowOptions = {
  allowNew?: boolean
}

type WalletPopupState = {
  pendingSignUrl: string | null
  signatureError: string | null
  blocked: boolean
  blockedUrl: string | null
  blockedContext: string | null
}

type WalletPopupAction =
  | { type: "setPending"; payload: string | null }
  | { type: "setSignatureError"; payload: string | null }
  | { type: "flagBlocked"; payload: { url: string | null; context: string } }
  | { type: "clearBlocked" }
  | { type: "reset" }

const initialWalletPopupState: WalletPopupState = {
  pendingSignUrl: null,
  signatureError: null,
  blocked: false,
  blockedUrl: null,
  blockedContext: null,
}

function walletPopupReducer(state: WalletPopupState, action: WalletPopupAction): WalletPopupState {
  switch (action.type) {
    case "setPending":
      return { ...state, pendingSignUrl: action.payload }
    case "setSignatureError":
      return { ...state, signatureError: action.payload }
    case "flagBlocked":
      return {
        ...state,
        blocked: true,
        blockedUrl: action.payload.url,
        blockedContext: action.payload.context,
      }
    case "clearBlocked":
      return { ...state, blocked: false, blockedUrl: null, blockedContext: null }
    case "reset":
      return initialWalletPopupState
    default:
      return state
  }
}

export type WalletBlockedState = {
  blocked: boolean
  url: string | null
  context: string | null
}

export type UseWalletBridgeResult = {
  pendingSignUrl: string | null
  setPendingSignUrl: (url: string | null) => void
  signatureError: string | null
  setSignatureError: (message: string | null) => void
  blockedState: WalletBlockedState
  clearBlockedState: () => void
  acknowledgeUserAction: () => void
  openWalletWindow: (targetUrl: string | null, contextLabel: string, options?: WalletWindowOptions) => Window | null
  primeWalletWindow: () => Window | null
  closeWalletWindow: () => void
  reset: () => void
}

export default function useWalletBridge(): UseWalletBridgeResult {
  const walletWindowRef = useRef<Window | null>(null)
  const [walletWindowHeartbeat, setWalletWindowHeartbeat] = useState(0)
  const [walletPopupState, dispatchWalletPopup] = useReducer(walletPopupReducer, initialWalletPopupState)

  const bumpHeartbeat = useCallback(() => {
    setWalletWindowHeartbeat((value) => value + 1)
  }, [])

  const clearBlockedState = useCallback(() => {
    dispatchWalletPopup({ type: "clearBlocked" })
  }, [])

  const acknowledgeUserAction = useCallback(() => {
    clearBlockedState()
    bumpHeartbeat()
  }, [clearBlockedState, bumpHeartbeat])

  const setPendingSignUrl = useCallback(
    (url: string | null) => {
      dispatchWalletPopup({ type: "setPending", payload: url })
      if (url) {
        acknowledgeUserAction()
      }
    },
    [acknowledgeUserAction],
  )

  const setSignatureError = useCallback((message: string | null) => {
    dispatchWalletPopup({ type: "setSignatureError", payload: message })
  }, [])

  const flagWalletPopupBlocked = useCallback((url: string | null, contextLabel: string) => {
    dispatchWalletPopup({ type: "flagBlocked", payload: { url, context: contextLabel } })
  }, [])

  const renderWalletWindowPlaceholder = useCallback((walletWindow: Window | null, contextLabel: string) => {
    if (!walletWindow || typeof walletWindow.document === "undefined") {
      return
    }
    const label = contextLabel || "Stellar wallet"
    try {
      const doc = walletWindow.document
      doc.open()
      doc.write(`<!DOCTYPE html>
  <html lang="en">
    <head>
      <meta charset="utf-8" />
      <title>${label}</title>
      <style>
        body {
          margin: 0;
          min-height: 100vh;
          display: flex;
          align-items: center;
          justify-content: center;
          font-family: Arial, sans-serif;
          background: #111827;
          color: #f8fafc;
        }
        .panel {
          background: rgba(15, 23, 42, 0.9);
          border: 1px solid #475569;
          border-radius: 12px;
          padding: 32px;
          max-width: 360px;
          text-align: center;
          box-shadow: 0 15px 35px rgba(0, 0, 0, 0.4);
        }
        h1 {
          font-size: 1.25rem;
          margin-bottom: 0.5rem;
        }
        p {
          margin: 0.25rem 0;
          line-height: 1.4;
        }
      </style>
    </head>
    <body>
      <div class="panel">
        <h1>Preparing ${label}?</h1>
        <p>You can keep playing while we open the wallet.</p>
        <p>If your browser blocked this window, allow popups for this site and try again.</p>
      </div>
    </body>
  </html>`)
      doc.close()
    } catch (err) {
      console.warn("[Stellar] Unable to render wallet placeholder page:", err)
    }
  }, [])

  const closeWalletWindow = useCallback(() => {
    try {
      walletWindowRef.current?.close?.()
    } catch {}
    walletWindowRef.current = null
    bumpHeartbeat()
  }, [bumpHeartbeat])

  const openWalletWindow = useCallback(
    (targetUrl: string | null, contextLabel: string, options?: WalletWindowOptions) => {
      const allowNew = options?.allowNew ?? true
      if (!targetUrl) {
        console.warn("[Stellar] Cannot open wallet window; missing URL.")
        return null
      }
      if (typeof window === "undefined") {
        console.warn("[Stellar] Cannot open wallet window outside the browser environment.")
        return null
      }

      const existing = walletWindowRef.current
      if (existing && !existing.closed) {
        try {
          existing.location.href = targetUrl
          existing.focus?.()
          acknowledgeUserAction()
          return existing
        } catch (err) {
          console.warn("[Stellar] Failed to reuse wallet window, reopening...", err)
          try {
            existing.close()
          } catch {}
          walletWindowRef.current = null
        }
      }

      if (!allowNew) {
        console.warn("[Stellar] Wallet window is closed; please click Attach again to continue signing.")
        return null
      }
      const opened = window.open(targetUrl, WALLET_WINDOW_NAME, WALLET_WINDOW_FEATURES)
      if (opened) {
        walletWindowRef.current = opened
        acknowledgeUserAction()
        opened.focus?.()
        return opened
      }
      flagWalletPopupBlocked(targetUrl, contextLabel)
      console.warn("[Stellar] Browser blocked the wallet window; please enable popups or open manually:", targetUrl)
      return null
    },
    [acknowledgeUserAction, flagWalletPopupBlocked],
  )

  const primeWalletWindow = useCallback(() => {
    if (typeof window === "undefined") {
      return null
    }
    const existing = walletWindowRef.current
    if (existing && !existing.closed) {
      existing.focus?.()
      return existing
    }
    const opened = window.open("", WALLET_WINDOW_NAME, WALLET_WINDOW_FEATURES)
    if (opened) {
      walletWindowRef.current = opened
      renderWalletWindowPlaceholder(opened, "Stellar wallet")
      try {
        opened.blur?.()
        window.focus?.()
      } catch {}
      clearBlockedState()
      return opened
    }
    console.warn(
      "[Stellar] Browser blocked the wallet window; please enable popups or open manually from the logged URL.",
    )
    flagWalletPopupBlocked(null, "Stellar wallet")
    return null
  }, [clearBlockedState, flagWalletPopupBlocked, renderWalletWindowPlaceholder])

  useEffect(() => {
    if (!walletPopupState.blocked) return
    if (typeof window === "undefined") return
    const walletWindow = walletWindowRef.current
    if (walletWindow && !walletWindow.closed) {
      clearBlockedState()
    }
  }, [walletPopupState.blocked, walletWindowHeartbeat, clearBlockedState])

  useEffect(() => {
    if (!walletPopupState.blocked) return
    if (typeof window === "undefined") return

    let attempts = 0
    let cancelled = false
    let timeoutId: number | null = null

    const scheduleNext = () => {
      if (cancelled) return
      timeoutId = window.setTimeout(tryClaimWindow, 600)
    }

    const tryClaimWindow = () => {
      if (cancelled || !walletPopupState.blocked) return
      if (attempts >= 6) return

      attempts += 1
      let reopened: Window | null = null
      try {
        reopened = window.open("", WALLET_WINDOW_NAME, WALLET_WINDOW_FEATURES)
      } catch {
        reopened = null
      }

      if (reopened && !reopened.closed) {
        const url = reopened.location?.href
        if (url && url !== "about:blank") {
          walletWindowRef.current = reopened
          clearBlockedState()
          return
        }
        reopened.close()
      }

      scheduleNext()
    }

    scheduleNext()

    return () => {
      cancelled = true
      if (timeoutId !== null && typeof window !== "undefined") {
        window.clearTimeout(timeoutId)
      }
    }
  }, [walletPopupState.blocked, clearBlockedState])

  const reset = useCallback(() => {
    dispatchWalletPopup({ type: "reset" })
    closeWalletWindow()
  }, [closeWalletWindow])

  return {
    pendingSignUrl: walletPopupState.pendingSignUrl,
    setPendingSignUrl,
    signatureError: walletPopupState.signatureError,
    setSignatureError,
    blockedState: {
      blocked: walletPopupState.blocked,
      url: walletPopupState.blockedUrl,
      context: walletPopupState.blockedContext,
    },
    clearBlockedState,
    acknowledgeUserAction,
    openWalletWindow,
    primeWalletWindow,
    closeWalletWindow,
    reset,
  }
}
