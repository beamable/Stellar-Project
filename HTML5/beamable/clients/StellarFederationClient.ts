/**
 * ⚠️ THIS FILE IS AUTO-GENERATED. DO NOT EDIT MANUALLY.
 * All manual edits will be lost when this file is regenerated.
 */

import { BeamMicroServiceClient, type BeamBase } from 'beamable-sdk';
import type * as Types from './types';

declare module 'beamable-sdk' {
  interface BeamBase {
    /**
     * Access the StellarFederation microservice.
     * @remarks Before accessing this property, register it first via the `use` method.
     * @example
     * ```ts
     * // client-side:
     * beam.use(StellarFederationClient);
     * beam.stellarFederationClient.serviceName;
     * // server-side:
     * beamServer.use(StellarFederationClient);
     * beamServer.stellarFederationClient.serviceName;
     * ```
     */
    stellarFederationClient: StellarFederationClient;
  }
}

export class StellarFederationClient extends BeamMicroServiceClient {
  readonly federationIds = {
    StellarIdentity: "StellarIdentity",
    StellarExternalIdentity: "StellarExternalIdentity"
  } as const;
  
  constructor(
    beam: BeamBase
  ) {
    super(beam);
  }
  
  get serviceName(): string {
    return "StellarFederation";
  }
  
  async getRealmAccount(): Promise<string> {
    return this.request({
      endpoint: "GetRealmAccount",
      withAuth: true
    });
  }
  
  async generateRealmAccount(): Promise<string> {
    return this.request({
      endpoint: "GenerateRealmAccount",
      withAuth: true
    });
  }
  
  async initializeContentContracts(): Promise<void> {
    return this.request({
      endpoint: "InitializeContentContracts",
      withAuth: true
    });
  }
  
  async stellarConfiguration(): Promise<Types.ConfigurationResponse> {
    return this.request({
      endpoint: "StellarConfiguration",
      withAuth: true
    });
  }
  
  async createAccount(): Promise<Types.AccountResponse> {
    return this.request({
      endpoint: "CreateAccount",
      withAuth: true
    });
  }
  
  async getAccount(): Promise<Types.AccountResponse> {
    return this.request({
      endpoint: "GetAccount",
      withAuth: true
    });
  }
  
  async externalAddress(params: Types.AuthAddressCallbackRequestArgs): Promise<void> {
    return this.request({
      endpoint: "ExternalAddress",
      payload: params,
      withAuth: true
    });
  }
  
  async externalSignature(params: Types.AuthSignatureCallbackRequestArgs): Promise<void> {
    return this.request({
      endpoint: "ExternalSignature",
      payload: params,
      withAuth: true
    });
  }
  
  async jobs(params: Types.JobsRequestArgs): Promise<Types.SchedulerJobResponse> {
    return this.request({
      endpoint: "Jobs",
      payload: params,
      withAuth: true
    });
  }
  
  async blockProcessor(): Promise<void> {
    return this.request({
      endpoint: "BlockProcessor",
      withAuth: true
    });
  }
  
  async getListings(params: Types.GetListingsRequestArgs): Promise<Types.GetListingsResponse> {
    return this.request({
      endpoint: "GetListings",
      payload: params,
      withAuth: true
    });
  }
  
  async updateCurrency(params: Types.UpdateCurrencyRequestArgs): Promise<void> {
    return this.request({
      endpoint: "UpdateCurrency",
      payload: params,
      withAuth: true
    });
  }
  
  async addItem(params: Types.AddItemRequestArgs): Promise<void> {
    return this.request({
      endpoint: "AddItem",
      payload: params,
      withAuth: true
    });
  }
  
  async addUniqueItem(params: Types.AddUniqueItemRequestArgs): Promise<boolean> {
    return this.request({
      endpoint: "AddUniqueItem",
      payload: params,
      withAuth: true
    });
  }
  
  async removeItem(params: Types.RemoveItemRequestArgs): Promise<void> {
    return this.request({
      endpoint: "RemoveItem",
      payload: params,
      withAuth: true
    });
  }
  
  async updateItems(params: Types.UpdateItemsRequestArgs): Promise<void> {
    return this.request({
      endpoint: "UpdateItems",
      payload: params,
      withAuth: true
    });
  }
  
  async updateInventory(params: Types.UpdateInventoryRequestArgs): Promise<void> {
    return this.request({
      endpoint: "UpdateInventory",
      payload: params,
      withAuth: true
    });
  }
  
  async purchaseBall(params: Types.PurchaseBallRequestArgs): Promise<void> {
    return this.request({
      endpoint: "PurchaseBall",
      payload: params,
      withAuth: true
    });
  }
}
