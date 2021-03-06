<template>
    <div>
        <b-modal
            id="rating-modal"
            ref="rating-modal"
            v-model="isVisible"
            title="Rating"
            size="md"
            header-bg-variant="primary"
            header-text-variant="light"
            footer-class="modal-footer"
            no-close-on-backdrop
            hide-header-close
            no-close-on-esc
            centered
        >
            <b-row class="text-center">
                <b-col>
                    {{ question }}
                </b-col>
            </b-row>
            <b-row class="text-center px-2 pt-3">
                <b-col>
                    <b-form-rating
                        v-model="ratingValue"
                        variant="warning"
                        class="mb-2"
                        no-border
                        size="lg"
                        @change="handleRating(ratingValue)"
                    ></b-form-rating>
                </b-col>
            </b-row>
            <template v-slot:modal-footer>
                <b-row>
                    <b-col>
                        <b-button
                            id="skipButton"
                            variant="outline-primary"
                            @click="handleRating(0, true)"
                            >Skip</b-button
                        >
                    </b-col>
                </b-row>
            </template>
        </b-modal>
    </div>
</template>
<script lang="ts">
import Vue from "vue";
import { Component, Prop, Emit } from "vue-property-decorator";
import { ILogger, IUserRatingService } from "@/services/interfaces";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";

@Component
export default class RatingComponent extends Vue {
    private logger: ILogger = container.get(SERVICE_IDENTIFIER.Logger);
    private question: string =
        "Did the Health Gateway improve your access to health information today? Please provide a rating.";
    private ratingValue: number = 0;
    private isVisible: boolean = false;

    public showModal() {
        this.isVisible = true;
    }

    public hideModal() {
        this.isVisible = false;
    }

    private handleRating(value: number, skip: boolean = false) {
        const ratingService: IUserRatingService = container.get(
            SERVICE_IDENTIFIER.UserRatingService
        );
        this.logger.debug(
            `submitting rating: ratingValue = ${value}, skip = ${skip} ...`
        );
        ratingService
            .submitRating({ ratingValue: value, skip: skip })
            .then(() => {
                this.logger.debug(`submitRating with success.`);
            })
            .catch((err) => {
                this.logger.error(`submitRating with error: ${err}`);
            })
            .finally(() => {
                this.hideModal();
                this.onClose();
            });
    }

    @Emit()
    public onClose() {
        return;
    }
}
</script>