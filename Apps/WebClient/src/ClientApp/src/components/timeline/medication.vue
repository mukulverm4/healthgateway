<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";

$radius: 15px;

.timelineCard {
    border-radius: $radius $radius $radius $radius;
    border-color: $soft_background;
    border-style: solid;
    border-width: 2px;
}

.entryTitle {
    background-color: $soft_background;
    color: $primary;
    padding: 13px 15px;
    font-weight: bold;
    margin-right: -1px;
    border-radius: 0px $radius 0px 0px;
}

.icon {
    background-color: $primary;
    color: white;
    text-align: center;
    padding: 10px 0;
    border-radius: $radius 0px 0px 0px;
}

.leftPane {
    width: 60px;
    max-width: 60px;
}

.detailsButton {
    padding: 0px;
}

.detailSection {
    margin-top: 15px;
}

.commentButton {
    border-radius: $radius;
}

.newComment {
    border-radius: $radius;
}

.collapsed > .when-opened,
:not(.collapsed) > .when-closed {
    display: none;
}
</style>

<template>
    <b-col class="timelineCard">
        <b-row class="entryHeading">
            <b-col class="icon leftPane">
                <font-awesome-icon
                    :icon="entryIcon"
                    size="2x"
                ></font-awesome-icon>
            </b-col>
            <b-col class="entryTitle">
                {{ entry.medication.brandName }}
            </b-col>
        </b-row>
        <b-row class="my-2">
            <b-col class="leftPane"></b-col>
            <b-col>
                <b-row>
                    <b-col>
                        {{ entry.medication.genericName }}
                    </b-col>
                </b-row>
                <b-row>
                    <b-col>
                        <div class="d-flex flex-row-reverse">
                            <b-btn
                                v-b-toggle="
                                    'entryDetails-' + index + '-' + datekey
                                "
                                variant="link"
                                class="detailsButton"
                                @click="toggleDetails(entry)"
                            >
                                <span class="when-opened">
                                    <font-awesome-icon
                                        icon="chevron-up"
                                        aria-hidden="true"
                                    ></font-awesome-icon
                                ></span>
                                <span class="when-closed">
                                    <font-awesome-icon
                                        icon="chevron-down"
                                        aria-hidden="true"
                                    ></font-awesome-icon
                                ></span>
                                <span v-if="detailsVisible">Hide Details</span>
                                <span v-else>View Details</span>
                            </b-btn>
                        </div>
                        <b-collapse
                            :id="'entryDetails-' + index + '-' + datekey"
                            v-model="detailsVisible"
                        >
                            <div v-if="detailsLoaded">
                                <div class="detailSection">
                                    <div>
                                        <strong>Practitioner:</strong>
                                        {{ entry.practitionerSurname }}
                                    </div>
                                </div>
                                <div class="detailSection">
                                    <div>
                                        <strong>Quantity:</strong>
                                        {{ entry.medication.quantity }}
                                    </div>
                                    <div>
                                        <strong>Strength:</strong>
                                        {{ entry.medication.strength }}
                                        {{ entry.medication.strengthUnit }}
                                    </div>
                                    <div>
                                        <strong>Form:</strong>
                                        {{ entry.medication.form }}
                                    </div>
                                    <div>
                                        <strong>Manufacturer:</strong>
                                        {{ entry.medication.manufacturer }}
                                    </div>
                                </div>
                                <div class="detailSection">
                                    <strong
                                        >{{
                                            entry.medication.isPin
                                                ? "PIN"
                                                : "DIN"
                                        }}:</strong
                                    >
                                    {{ entry.medication.din }}
                                </div>
                                <div class="detailSection">
                                    <div>
                                        <strong>Filled At:</strong>
                                    </div>
                                    <div>
                                        {{ entry.pharmacy.name }}
                                    </div>
                                    <div>
                                        {{ entry.pharmacy.address }}
                                    </div>
                                    <div
                                        v-if="
                                            entry.pharmacy.phoneType !=
                                            faxPhoneType
                                        "
                                    >
                                        {{
                                            formatPhoneNumber(
                                                entry.pharmacy.phoneNumber
                                            )
                                        }}
                                    </div>
                                    <div
                                        class="detailSection border border-dark p-2 small"
                                    >
                                        <div>
                                            <strong>Directions for Use:</strong>
                                        </div>
                                        <div class="pt-2">
                                            {{ entry.directions }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div v-else-if="isLoading">
                                <div class="d-flex align-items-center">
                                    <strong>Loading...</strong>
                                    <b-spinner class="ml-5"></b-spinner>
                                </div>
                            </div>
                            <div v-else-if="hasErrors" class="pt-1">
                                <b-alert :show="hasErrors" variant="danger">
                                    <h5>Error</h5>
                                    <span
                                        >An unexpected error occured while
                                        processing the request.</span
                                    >
                                </b-alert>
                            </div>
                        </b-collapse>
                    </b-col>
                </b-row>
            </b-col>
        </b-row>
        <CommentSection :parent-entry="entry"></CommentSection>
    </b-col>
</template>

<script lang="ts">
import Vue from "vue";
import container from "@/plugins/inversify.config";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import { ILogger } from "@/services/interfaces";
import Pharmacy, { PhoneType } from "@/models/pharmacy";
import MedicationTimelineEntry from "@/models/medicationTimelineEntry";
import CommentSectionComponent from "@/components/timeline/commentSection.vue";
import { Component, Prop } from "vue-property-decorator";
import { Action, Getter, State } from "vuex-class";
import { IconDefinition, faPills } from "@fortawesome/free-solid-svg-icons";
import MedicationResult from "@/models/medicationResult";

@Component({
    components: {
        CommentSection: CommentSectionComponent,
    },
})
export default class MedicationTimelineComponent extends Vue {
    private logger: ILogger = container.get(SERVICE_IDENTIFIER.Logger);
    @Prop() entry!: MedicationTimelineEntry;
    @Prop() index!: number;
    @Prop() datekey!: string;

    @Action("getMedication", { namespace: "medication" })
    getMedication!: (params: { din: string }) => Promise<MedicationResult>;

    private faxPhoneType: PhoneType = PhoneType.Fax;
    private isLoadingMedication: boolean = false;
    private isLoadingPharmacy: boolean = false;
    private hasErrors: boolean = false;
    private medicationLoaded: boolean = false;
    private detailsVisible: boolean = false;

    private get detailsLoaded(): boolean {
        return this.medicationLoaded;
    }

    private get isLoading(): boolean {
        return this.isLoadingMedication || this.isLoadingPharmacy;
    }

    private get entryIcon(): IconDefinition {
        return faPills;
    }

    private toggleDetails(medicationEntry: MedicationTimelineEntry): void {
        this.detailsVisible = !this.detailsVisible;
        this.hasErrors = false;

        if (!this.detailsVisible) {
            return;
        }

        // Load medication details
        if (!this.medicationLoaded) {
            this.isLoadingMedication = true;
            this.logger.debug(
                `Loading Medication Entry : ${JSON.stringify(medicationEntry)}`
            );
            var medicationPromise = this.getMedication({
                din: medicationEntry.medication.din,
            })
                .then((result) => {
                    if (result) {
                        medicationEntry.medication.populateFromModel(result);
                    }
                    this.medicationLoaded = true;
                    this.isLoadingMedication = false;
                })
                .catch((err) => {
                    this.logger.error(
                        `Error loading medication details : ${err}`
                    );
                    this.hasErrors = true;
                    this.isLoadingMedication = false;
                });
        }
    }

    private formatPhoneNumber(phoneNumber: string): string {
        phoneNumber = phoneNumber || "";
        return phoneNumber
            .replace(/[^0-9]/g, "")
            .replace(/(\d{3})(\d{3})(\d{4})/, "($1) $2-$3");
    }
}
</script>
