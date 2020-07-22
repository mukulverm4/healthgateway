<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";
.column-wrapper {
    border: 1px;
}

#pageTitle {
    color: $primary;
}

#pageTitle hr {
    border-top: 2px solid $primary;
}

.sortContainer {
    text-align: right;
}

.dateBreakLine {
    border-top: dashed 2px $primary;
}

.date {
    padding-top: 0px;
    color: $primary;
    font-size: 1.3em;
}

.has-filter .form-control {
    padding-left: 2.375rem;
}

.has-filter .form-control-feedback {
    position: absolute;
    z-index: 2;
    display: block;
    width: 2.375rem;
    height: 2.375rem;
    line-height: 2.375rem;
    text-align: center;
    pointer-events: none;
    color: #aaa;
    padding: 12px;
}

.btn-light {
    border-color: $primary;
    color: $primary;
}
</style>
<template>
    <div>
        <LoadingComponent :is-loading="isLoading"></LoadingComponent>
        <b-row class="my-3 fluid justify-content-md-center">
            <b-col class="col-12 col-md-1 col-lg-1 column-wrapper no-print">
            </b-col>
            <b-col
                id="timeline"
                class="col-12 col-md-8 col-lg-6 column-wrapper"
            >
                <b-alert
                    :show="hasErrors"
                    dismissible
                    variant="danger"
                    class="no-print"
                >
                    <h4>Error</h4>
                    <span
                        >An unexpected error occured while processing the
                        request.</span
                    >
                </b-alert>
                <b-alert
                    :show="hasNewTermsOfService"
                    dismissible
                    variant="info"
                    class="no-print"
                >
                    <h4>Updated Terms of Service</h4>
                    <span>
                        The Terms of Service have been updated since your last
                        login. You can review them
                        <router-link
                            id="termsOfServiceLink"
                            variant="primary"
                            to="/termsOfService"
                        >
                            here </router-link
                        >.
                    </span>
                </b-alert>
                <b-alert
                    :show="unverifiedEmail"
                    dismissible
                    variant="info"
                    class="no-print"
                >
                    <h4>Unverified email</h4>
                    <span>
                        Your email has not been verified. Please check your
                        inbox or junk folder for an email from Health Gateway.
                        You can also edit your profile or resend the email from
                        the
                        <router-link
                            id="profilePageLink"
                            variant="primary"
                            to="/profile"
                        >
                            profile page</router-link
                        >.
                    </span>
                </b-alert>
                <div id="pageTitle">
                    <h1 id="subject">Health Insights</h1>
                    <hr />
                </div>
                <br />
                <div v-if="!isLoading">
                    <!--
          <div id="timeData">
            <b-row v-for="dateGroup in dateGroups" :key="dateGroup.key">
              <b-col cols="auto">
                <div class="date">{{ getHeadingDate(dateGroup.date) }}</div>
              </b-col>
              <div
                v-for="(entry, index) in dateGroup.entries"
                :key="entry.type + '-' + entry.id"
                :datekey="dateGroup.key"
                :index="index"
              >
                {{ entry.medication.atcNumber }}
              </div>
            </b-row>
          </div>
          -->
                    <h2>Medication count over time</h2>
                    <hr />
                    <LineComponent
                        :chartdata="timeChartData"
                        :options="chartOptions"
                    />
                    <br />
                    <h2>Medication classification by ATC</h2>
                    <hr />
                    <BarComponent
                        :chartdata="categoryChartData"
                        :options="chartOptions"
                    />
                </div>
            </b-col>
        </b-row>
        <ProtectiveWordComponent
            ref="protectiveWordModal"
            :error="protectiveWordAttempts > 1"
            :is-loading="isLoading"
            @submit="onProtectiveWordSubmit"
            @cancel="onProtectiveWordCancel"
        />
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Ref, Watch } from "vue-property-decorator";
import { Action, Getter } from "vuex-class";
import moment from "moment";
import { Route } from "vue-router";
import EventBus from "@/eventbus";
import { WebClientConfiguration } from "@/models/configData";
import { IMedicationService } from "@/services/interfaces";
import container from "@/plugins/inversify.config";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import { ResultType } from "@/constants/resulttype";
import User from "@/models/user";
import TimelineEntry, { EntryType, DateGroup } from "@/models/timelineEntry";
import MedicationTimelineEntry from "@/models/medicationTimelineEntry";
import MedicationStatement from "@/models/medicationStatement";
import UserNote from "@/models/userNote";
import RequestResult from "@/models/requestResult";
import { IconDefinition, faSearch } from "@fortawesome/free-solid-svg-icons";

import TimelineLoadingComponent from "@/components/timelineLoading.vue";
import ProtectiveWordComponent from "@/components/modal/protectiveWord.vue";
import CovidModalComponent from "@/components/modal/covid.vue";
import HealthlinkSidebarComponent from "@/components/timeline/healthlink.vue";
import NoteTimelineComponent from "@/components/timeline/note.vue";
import {
    LaboratoryOrder,
    LaboratoryResult,
    LaboratoryReport,
} from "@/models/laboratory";
import MedicationResult from "@/models/medicationResult";
import { state } from "../store/modules/auth/auth";
import { Dictionary } from "vue-router/types/router";

import LineComponent from "@/components/timeline/line.vue";
import BarComponent from "@/components/timeline/bar.vue";

const namespace: string = "user";

// Register the router hooks with their names
Component.registerHooks(["beforeRouteLeave"]);

@Component({
    components: {
        TimelineLoadingComponent,
        ProtectiveWordComponent,
        CovidModalComponent,
        HealthlinkComponent: HealthlinkSidebarComponent,
        NoteTimelineComponent,
        LineComponent,
        BarComponent,
    },
})
export default class TherapeuticClassificationComponent extends Vue {
    @Getter("user", { namespace }) user!: User;
    @Getter("webClient", { namespace: "config" })
    config!: WebClientConfiguration;
    @Action("getMedication", { namespace: "medication" }) getMedication!: ({
        din: string,
    }: any) => Promise<MedicationResult>;

    @Action("getMedicationList", { namespace: "medication" })
    getMedicationList!: (params: {
        dinList: string[];
    }) => Promise<Dictionary<MedicationResult>>;

    private filterText: string = "";
    private timelineEntries: MedicationTimelineEntry[] = [];
    private visibleTimelineEntries: MedicationTimelineEntry[] = [];
    private isMedicationLoading: boolean = true;
    private isMedicationDetailLoading: boolean = false;
    private medicationDictionary: Dictionary<MedicationResult> = {};
    private windowWidth: number = 0;

    private hasErrors: boolean = false;
    private idleLogoutWarning: boolean = false;
    private protectiveWordAttempts: number = 0;

    private categoryChartData: {
        labels: any;
        datasets: any;
    } | null = null;
    private timeChartData: any | null = null;
    private chartOptions: {} = { responsive: true, maintainAspectRatio: false };

    private eventBus = EventBus;

    @Ref("protectiveWordModal")
    readonly protectiveWordModal!: ProtectiveWordComponent;

    private mounted() {
        this.fetchMedicationStatements();
        let self = this;
        this.eventBus.$on("idleLogoutWarning", function (isVisible: boolean) {
            self.idleLogoutWarning = isVisible;
        });
    }

    private beforeRouteLeave(to: Route, from: Route, next: any) {
        next();
    }

    private get unverifiedEmail(): boolean {
        return !this.user.verifiedEmail && this.user.hasEmail;
    }

    private get hasNewTermsOfService(): boolean {
        return this.user.hasTermsOfServiceUpdated;
    }

    private get isLoading(): boolean {
        return this.isMedicationLoading;
    }

    private get isMedicationEnabled(): boolean {
        return (
            this.config.modules["MedicationHistory"] ||
            this.config.modules["Medication"]
        );
    }

    private fetchMedicationStatements(protectiveWord?: string) {
        const medicationService: IMedicationService = container.get(
            SERVICE_IDENTIFIER.MedicationService
        );
        this.isMedicationLoading = true;
        const isOdrEnabled = this.config.modules["MedicationHistory"];
        let promise: Promise<RequestResult<MedicationStatement[]>>;
        if (isOdrEnabled) {
            promise = medicationService.getPatientMedicationStatementHistory(
                this.user.hdid,
                protectiveWord
            );
        } else {
            promise = medicationService.getPatientMedicationStatements(
                this.user.hdid,
                protectiveWord
            );
        }

        promise
            .then((results) => {
                if (results.resultStatus == ResultType.Success) {
                    this.protectiveWordAttempts = 0;
                    return this.getMedicationDetails(
                        results.resourcePayload
                    ).then(() => {
                        // Add the medication entries to the timeline list
                        for (let result of results.resourcePayload) {
                            let medicationEntry = new MedicationTimelineEntry(
                                result
                            );
                            this.timelineEntries.push(medicationEntry);
                        }
                        this.mapMedicationResults();
                        this.sortEntries();

                        this.prepareCaterogryChart();
                        this.prepareMonthlyChart();
                    });
                } else if (results.resultStatus == ResultType.Protected) {
                    this.protectiveWordModal.showModal();

                    this.protectiveWordAttempts++;
                } else {
                    console.log(
                        "Error returned from the medication statements call: " +
                            results.resultMessage
                    );
                    this.hasErrors = true;
                }
            })
            .catch((err) => {
                this.hasErrors = true;
                console.log(err);
            })
            .finally(() => {
                this.isMedicationLoading = false;
            });
    }

    private getMedicationDetails(
        statements: MedicationStatement[]
    ): Promise<void> {
        this.hasErrors = false;

        let dinList = statements.reduce(function (result: string[], statement) {
            if (statement.medicationSumary.din) {
                result.push(statement.medicationSumary.din);
            }
            return result;
        }, []);

        // Remove repeated dins
        dinList = [...new Set(dinList)];

        // Load medication details
        this.isMedicationDetailLoading = true;
        return this.getMedicationList({
            dinList: dinList,
        })
            .then((result) => {
                if (result) {
                    this.medicationDictionary = result;
                    //medicationEntry.medication.populateFromModel(result);
                }
            })
            .catch((err) => {
                console.log("Error loading medication details");
                console.log(err);
                this.hasErrors = true;
            })
            .finally(() => {
                this.isMedicationDetailLoading = false;
            });
    }

    private mapMedicationResults(): void {
        this.hasErrors = false;

        this.timelineEntries.forEach((e) => {
            let paddedDin = e.medication.din.padStart(8, "0");
            let medicationDetail = this.medicationDictionary[paddedDin];
            if (medicationDetail) {
                e.medication.populateFromModel(medicationDetail);
            }
        });
    }

    private onProtectiveWordSubmit(value: string) {
        this.fetchMedicationStatements(value);
    }

    private onProtectiveWordCancel() {
        // Does nothing as it won't be able to fetch pharmanet data.
        console.log("protective word cancelled");
    }

    private getHeadingDate(date: Date): string {
        return moment(date).format("ll");
    }

    private get dateGroups(): DateGroup[] {
        if (this.visibleTimelineEntries.length === 0) {
            return [];
        }
        let groups = this.visibleTimelineEntries.reduce<
            Record<string, TimelineEntry[]>
        >((groups, entry) => {
            // Get the string version of the date and get the date
            const date = new Date(entry.date).setHours(0, 0, 0, 0);

            // Create a new group if it the date doesnt exist in the map
            if (!groups[date]) {
                groups[date] = [];
            }
            groups[date].push(entry);
            return groups;
        }, {});
        let groupArrays = Object.keys(groups).map<DateGroup>((dateKey) => {
            return {
                key: dateKey,
                date: new Date(groups[dateKey][0].date),
                entries: groups[
                    dateKey
                ].sort((a: TimelineEntry, b: TimelineEntry) =>
                    a.type > b.type ? 1 : a.type < b.type ? -1 : 0
                ),
            };
        });
        return groupArrays;
    }

    private sortGroup(groupArrays: any[]) {
        groupArrays.sort((a, b) =>
            a.date > b.date ? -1 : a.date < b.date ? 1 : 0
        );
        return groupArrays;
    }

    private getVisibleCount(): number {
        return this.visibleTimelineEntries.length;
    }

    private getTotalCount(): number {
        return this.timelineEntries.length;
    }

    private sortEntries() {
        this.timelineEntries.sort((a, b) =>
            a.date > b.date ? -1 : a.date < b.date ? 1 : 0
        );
        this.visibleTimelineEntries = this.timelineEntries;
    }

    private getCategories(): any {
        return this.visibleTimelineEntries.reduce<any>((categories, entry) => {
            // Create a new group if it the date doesnt exist in the map
            let atcCategory = this.getCategoryDescription(
                entry.medication.atcNumber[0]
            );
            if (!categories[atcCategory]) {
                categories[atcCategory] = 0;
            }
            categories[atcCategory] += 1;
            return categories;
        }, {});
    }

    private getCategoryDescription(firstLevel: string): string {
        switch (firstLevel) {
            case "A":
                return "Alimentary tract and metabolism";
            case "B":
                return "Blood and blood forming organs";
            case "C":
                return "Cardiovascular system";
            case "D":
                return "Dermatologicals";
            case "G":
                return "Genito urinary system and sex hormones";
            case "H":
                return "Systemic hormonal preparations, excluding sex hormones and insulins";
            case "J":
                return "Antiinfective for systemic use";
            case "L":
                return "Antineoplastic and immunomodulating agents";
            case "M":
                return "Musculo-skeletal system";
            case "N":
                return "Nervous system";
            case "P":
                return "Antiparasitic products, insecticides and repellents";
            case "R":
                return "Respiratory system";
            case "S":
                return "Sensory organs";
            case "V":
                return "Various";
            case "X":
                return "PIN - BC Provincial";
            default:
                return "Unknown";
        }
    }

    private getMonthsBetweenDates(start: Date, end: Date) {
        console.log(start, end);
        var startDate = moment(start);
        var endDate = moment(end);

        var result = [];

        if (endDate.isBefore(startDate)) {
            throw "End date must be greated than start date.";
        }

        while (startDate.isBefore(endDate)) {
            result.push(startDate.format("YYYY-MM-01"));
            startDate.add(1, "month");
        }
        return result;
    }

    private prepareMonthlyChart() {
        let ascendingEntries = this.timelineEntries.reverse();

        let months: string[] = this.getMonthsBetweenDates(
            ascendingEntries[0].date,
            ascendingEntries[ascendingEntries.length - 1].date
        );

        let entryIndex = 0;
        let monthCounter: number[] = [];
        for (let monthIndex = 0; monthIndex < months.length; monthIndex++) {
            let currentMonth = months[monthIndex];
            monthCounter.push(0);

            while (entryIndex < ascendingEntries.length) {
                let entry = ascendingEntries[entryIndex];
                if (moment(currentMonth).isSame(entry.date, "month")) {
                    monthCounter[monthIndex] += 1;
                    entryIndex++;
                } else {
                    break;
                }
            }
        }

        console.log(monthCounter);

        /*let groups = this.visibleTimelineEntries.reduce<any>((groups, entry) => {
      // Get the string version of the date and get the date
      //const date = (entry.date).split("T")[0];
      const date = new Date(entry.date).setHours(0, 0, 0, 0);
      // Create a new group if it the date doesnt exist in the map
      if (months[date]) {
        groups[date] = 0;
      }
      groups[date].push(entry);
      return groups;
    }, {});*/

        this.timeChartData = {
            labels: [],
            datasets: [
                {
                    label: "Medications",
                    backgroundColor: "#f87979",
                    data: [],
                },
            ],
        };

        for (let i = 0; i < months.length; i++) {
            this.timeChartData.labels.push(
                moment(months[i]).format("MMM YYYY")
            );
            this.timeChartData.datasets[0].data.push(monthCounter[i]);
        }
    }

    private prepareCaterogryChart() {
        this.categoryChartData = {
            labels: [],
            datasets: [
                {
                    label: "Views by Category",
                    data: [],
                    backgroundColor: [
                        "#0074D9",
                        "#FF4136",
                        "#2ECC40",
                        "#FF851B",
                        "#7FDBFF",
                        "#B10DC9",
                        "#FFDC00",
                        "#001f3f",
                        "#39CCCC",
                        "#01FF70",
                        "#85144b",
                        "#F012BE",
                        "#3D9970",
                        "#111111",
                        "#AAAAAA",
                    ],
                },
            ],
        };

        let cats = this.getCategories();
        console.log(cats);

        for (let cat in cats) {
            this.categoryChartData.labels.push(cat);
            let med = cats[cat];
            this.categoryChartData.datasets[0].data.push(med);
        }
    }
}
</script>
