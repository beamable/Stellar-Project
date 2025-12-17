/**
 * ƒsÿ‹,? THIS FILE IS AUTO-GENERATED. DO NOT EDIT MANUALLY.
 * All manual edits will be lost when this file is regenerated.
 */

export type ConfigurationResponse = {
  network: string
  walletConnectBridgeUrl: string
}

export type AccountResponse = {
  wallet: string
  created: boolean
}

export type SchedulerJobResponse = {
  names: string[]
}

export type JobsRequestArgs = {
  enable: boolean
}

export type ClientDataView = {
  name: string
  value: string
}

export type OfferPriceView = {
  symbol: string
  type: string
  amount: number
  schedule: number[]
}

export type ObtainCurrencyView = {
  symbol: string
  amount: bigint | string
}

export type ItemPropertyView = {
  name: string
  value: string
}

export type ObtainItemsView = {
  contentId: string
  properties: ItemPropertyView[]
}

export type OfferView = {
  symbol: string
  titles: string[]
  descriptions: string[]
  price: OfferPriceView
  obtainCurrency: ObtainCurrencyView[]
  obtainItems: ObtainItemsView[]
}

export type ListingView = {
  symbol: string
  active: boolean
  secondsActive: bigint | string
  secondsRemain: bigint | string
  purchasesRemain: number
  cooldown: number
  clientData: ClientDataView[]
  offer: OfferView
}

export type StoreView = {
  title: string
  symbol: string
  nextDeltaSeconds: bigint | string
  secondsRemain: bigint | string
  listings: ListingView[]
}

export type GetListingsResponse = {
  stores: StoreView[]
}

export type GetListingsRequestArgs = {
  storeId: string
}

export type UpdateCurrencyRequestArgs = {
  currencyContentId: string
  amount: number
}

export type StringStringMap = Record<string, string>

export type AddItemRequestArgs = {
  itemContentId: string
  properties?: StringStringMap | null
}

export type AddUniqueItemRequestArgs = {
  itemContentId: string
  properties?: StringStringMap | null
}

export type RemoveItemRequestArgs = {
  itemContentId: string
  instanceId: bigint | string
}

export type CropUpdateRequest = {
  ContentId: string
  InstanceId: bigint | string
  Properties: StringStringMap
}

export type UpdateItemsRequestArgs = {
  items: CropUpdateRequest[]
}

export type UpdateInventoryRequestArgs = {
  currencyContentId: string
  amount: number
  items: CropUpdateRequest[]
}

export type PurchaseBallRequestArgs = {
  purchaseId: string
}

export type AuthAddressCallbackRequestArgs = {
  address: string
  gamerTag: string
}

export type AuthSignatureCallbackRequestArgs = {
  address: string
  message: string
  signature: string
}

