<template>
    <v-layout class="fill-height">
        <LoadingComponent :is-loading="isLoading"></LoadingComponent>
        <v-row justify="center">
            <span v-if="isAuthenticated">Logging out...</span>
            <span v-else>You have been logged out</span>
        </v-row>
    </v-layout>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { State, Action, Getter } from "vuex-class";
import LoadingComponent from "@/components/core/Loading.vue";

const namespace: string = "auth";

@Component({
    components: {
        LoadingComponent
    }
})
export default class LoginView extends Vue {
    public name = "Dashboard";
    @Action("logout", { namespace }) private logout!: () => Promise<void>;
    @Getter("isAuthenticated", { namespace }) private isAuthenticated!: boolean;
    private isLoading: boolean = true;

    mounted() {
        this.isLoading = true;
        if (this.isAuthenticated) {
            this.logout().then(() => {
                this.isLoading = false;
            });
        } else {
            this.isLoading = false;
        }
    }
}
</script>
