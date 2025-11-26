/**
 * ⚠️ THIS FILE IS AUTO-GENERATED. DO NOT EDIT MANUALLY.
 * All manual edits will be lost when this file is regenerated.
 */

export type ConfigurationResponse = { 
  network: string; 
  walletConnectBridgeUrl: string; 
};

export type SendTestNotificationRequestArgs = { 
  message: string; 
};

export type UpdateCurrencyRequestArgs = { 
  currencyContentId: string; 
  amount: number; 
};

export type AddItemRequestArgs = { 
  itemContentId: string; 
};

export type RemoveItemRequestArgs = { 
  itemContentId: string; 
  instanceId: bigint | string; 
};

export type PurchaseBallRequestArgs = { 
  purchaseId: string; 
};
