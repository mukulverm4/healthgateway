﻿<template>
    <div class="row justify-content-center h-100 pt-5">
        <div class="col-lg-6 col-md-6 pt-2">
            <div
                v-if="!oidcIsAuthenticated"
                class="shadow-lg p-3 mb-5 bg-white rounded border"
            >
                <h3>
                    <strong>You signed out of your account</strong>
                </h3>
                <p>It's a good idea to close all browser windows.</p>
            </div>
            <div v-else>
                <h3>Signing out...</h3>
            </div>
            <RatingComponent ref="ratingComponent" :on-close="modalClosed()" />
        </div>
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Ref } from "vue-property-decorator";
import { Action, Getter } from "vuex-class";
import { WebClientConfiguration } from "@/models/configData";
import RatingComponent from "@/components/modal/rating.vue";
const namespace = "auth";

@Component({
    components: {
        RatingComponent: RatingComponent,
    },
})
export default class LogoutView extends Vue {
    @Action("signOutOidc", { namespace }) logout!: () => void;

    @Getter("oidcIsAuthenticated", { namespace }) oidcIsAuthenticated!: boolean;
    @Getter("webClient", { namespace: "config" })
    config!: WebClientConfiguration;

    @Ref("ratingComponent")
    readonly ratingComponent!: RatingComponent;

    private mounted() {
        this.ratingComponent.showModal();
    }

    private modalClosed() {
        if (this.oidcIsAuthenticated) {
            this.logout();
        }
        setTimeout(() => {
            if (this.$route.path == "/logout") {
                this.$router.push({ path: "/" });
            }
        }, Number(this.config.timeouts!.logoutRedirect));
    }
}
</script>
