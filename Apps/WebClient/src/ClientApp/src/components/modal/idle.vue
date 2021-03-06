﻿<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";
.modal-footer {
    justify-content: flex-start;
}
</style>
<template>
    <b-modal
        id="idle-modal"
        v-model="visible"
        header-bg-variant="primary"
        header-text-variant="light"
        :ok-only="true"
        title="Are you still there?"
        ok-title="I'm here!"
        centered
        @ok="refresh"
        @hidden="refresh"
    >
        <b-row>
            <b-col>
                You will be automatically logged out in {{ totalTime }} seconds.
            </b-col>
        </b-row>
    </b-modal>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Emit } from "vue-property-decorator";
import { Action } from "vuex-class";
import EventBus from "@/eventbus";
import User from "@/models/user";

@Component
export default class IdleComponent extends Vue {
    @Action("authenticateOidcSilent", { namespace: "auth" })
    authenticateOidcSilent!: () => Promise<void>;

    private readonly modalId: string = "idle-modal";
    private totalTime: number = 60;
    private visible: boolean = false;
    private timerHandle?: number;
    private timeoutHandle?: number;
    private eventBus = EventBus;

    @Emit()
    public show() {
        if (!this.visible) {
            this.$bvModal.show(this.modalId);
            var self = this;
            this.timerHandle = window.setInterval(() => self.countdown(), 1000);
            this.timeoutHandle = window.setTimeout(() => {
                this.eventBus.$emit("idleLogoutWarning", true);
                self.logout();
            }, 1000 * 60);
        }
    }
    @Emit()
    public logout() {
        this.$router.push("/logout");
    }

    private refresh(bvModalEvt: Event) {
        this.authenticateOidcSilent();
        this.reset();
    }

    private reset() {
        this.totalTime = 60;
        window.clearTimeout(this.timeoutHandle);
        window.clearInterval(this.timerHandle);
        this.eventBus.$emit("idleLogoutWarning", false);
    }

    private countdown() {
        this.totalTime--;
    }
}
</script>
