import { GetterTree } from "vuex";
import { RootState, ConfigState } from "@/models/storeState";
import {
  IdentityProviderConfiguration,
  OpenIdConnectConfiguration,
  ExternalClientConfiguration
} from "@/models/configData";

export const getters: GetterTree<ConfigState, RootState> = {
  identityProviders(state: ConfigState): IdentityProviderConfiguration[] {
    const { config } = state;
    const { identityProviders = [] } = config;
    return identityProviders;
  },
  openIdConnect(state: ConfigState): OpenIdConnectConfiguration {
    const { config } = state;
    const { openIdConnect } = config;
    return openIdConnect;
  },
  externalClient(state: ConfigState): ExternalClientConfiguration {
    const { config } = state;
    const { externalClient } = config;
    return externalClient;
  }
};
