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
    StellarIdentity: "StellarIdentity",
    StellarExternalIdentity: "StellarExternalIdentity",
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
  
  async stellarConfiguration(): Promise<Types.ConfigurationResponse> {
    return this.request({
      endpoint: "StellarConfiguration",
      withAuth: true
    });
  }
  
  async sendTestNotification(params: Types.SendTestNotificationRequestArgs): Promise<void> {
    return this.request({
      endpoint: "SendTestNotification",
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
  
  async removeItem(params: Types.RemoveItemRequestArgs): Promise<void> {
    return this.request({
      endpoint: "RemoveItem",
      payload: params,
      withAuth: true
    });
  }
  
  async externalAddress(): Promise<void> {
    return this.request({
      endpoint: "ExternalAddress",
      withAuth: true
    });
  }
  
  async externalSignature(): Promise<void> {
    return this.request({
      endpoint: "ExternalSignature",
      withAuth: true
    });
  }
}
