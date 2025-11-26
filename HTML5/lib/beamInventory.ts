import type {
  Beam,
  CommonResponse,
  CurrencyContentResponse,
  HttpResponse,
  InventoryQueryRequest,
  InventoryUpdateRequest,
  InventoryView,
  ItemContentResponse,
  MultipliersGetResponse,
  TransferRequest,
  EndTransactionRequest,
} from "beamable-sdk"
import {
  inventoryDeleteTransactionByObjectId,
  inventoryGetByObjectId,
  inventoryGetCurrencyBasic,
  inventoryGetItemsBasic,
  inventoryGetMultipliersByObjectId,
  inventoryPostByObjectId,
  inventoryPutByObjectId,
  inventoryPutPreviewByObjectId,
  inventoryPutProxyReloadByObjectId,
  inventoryPutTransferByObjectId,
} from "beamable-sdk"

/**
 * Lightweight wrappers over the WebSDK inventory API functions.
 * Source docs: C:\Beamable\Repos\Downloads\BeamWebSdk-htmlPages\inventory*.html
 */
type Maybe<T> = T | undefined

type InventoryScope = {
  objectId?: bigint | string
  scope?: string
  gamertag?: string
}

const currentPlayerId = (beam: Beam, objectId?: bigint | string) =>
  objectId ?? beam.player.id

const unwrap = async <T>(resPromise: Promise<HttpResponse<T>>) =>
  (await resPromise).data

export const fetchInventory = (
  beam: Beam,
  options: InventoryScope = {},
): Promise<InventoryView> => {
  const { objectId, scope, gamertag } = options
  return unwrap(
    inventoryGetByObjectId(
      beam.requester,
      currentPlayerId(beam, objectId),
      scope,
      gamertag,
    ),
  )
}

export const fetchItems = (
  beam: Beam,
  gamertag?: string,
): Promise<ItemContentResponse> =>
  unwrap(inventoryGetItemsBasic(beam.requester, gamertag))

export const fetchCurrencies = (
  beam: Beam,
  gamertag?: string,
): Promise<CurrencyContentResponse> =>
  unwrap(inventoryGetCurrencyBasic(beam.requester, gamertag))

export const fetchMultipliers = (
  beam: Beam,
  objectId?: bigint | string,
  gamertag?: string,
): Promise<MultipliersGetResponse> =>
  unwrap(
    inventoryGetMultipliersByObjectId(
      beam.requester,
      currentPlayerId(beam, objectId),
      gamertag,
    ),
  )

export const queryInventory = (
  beam: Beam,
  payload: InventoryQueryRequest,
  options: Pick<InventoryScope, "objectId" | "gamertag"> = {},
): Promise<InventoryView> =>
  unwrap(
    inventoryPostByObjectId(
      beam.requester,
      currentPlayerId(beam, options.objectId),
      payload,
      options.gamertag,
    ),
  )

export const previewUpdate = (
  beam: Beam,
  payload: InventoryUpdateRequest,
  options: Pick<InventoryScope, "objectId" | "gamertag"> = {},
): Promise<CommonResponse> =>
  unwrap(
    inventoryPutPreviewByObjectId(
      beam.requester,
      currentPlayerId(beam, options.objectId),
      payload,
      options.gamertag,
    ),
  )

export const applyUpdate = (
  beam: Beam,
  payload: InventoryUpdateRequest,
  options: Pick<InventoryScope, "objectId" | "gamertag"> = {},
): Promise<CommonResponse> =>
  unwrap(
    inventoryPutByObjectId(
      beam.requester,
      currentPlayerId(beam, options.objectId),
      payload,
      options.gamertag,
    ),
  )

export const transferInventory = (
  beam: Beam,
  payload: TransferRequest,
  options: Pick<InventoryScope, "objectId" | "gamertag"> = {},
): Promise<CommonResponse> =>
  unwrap(
    inventoryPutTransferByObjectId(
      beam.requester,
      currentPlayerId(beam, options.objectId),
      payload,
      options.gamertag,
    ),
  )

export const reloadProxy = (
  beam: Beam,
  options: Pick<InventoryScope, "objectId" | "gamertag"> = {},
): Promise<CommonResponse> =>
  unwrap(
    inventoryPutProxyReloadByObjectId(
      beam.requester,
      currentPlayerId(beam, options.objectId),
      options.gamertag,
    ),
  )

export const endTransaction = (
  beam: Beam,
  payload: EndTransactionRequest,
  options: Pick<InventoryScope, "objectId" | "gamertag"> = {},
): Promise<CommonResponse> =>
  unwrap(
    inventoryDeleteTransactionByObjectId(
      beam.requester,
      currentPlayerId(beam, options.objectId),
      payload,
      options.gamertag,
    ),
  )

