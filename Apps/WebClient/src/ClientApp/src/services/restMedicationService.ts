import { injectable } from "inversify";
import { Dictionary } from "vue-router/types/router";
import { IHttpDelegate, IMedicationService } from "@/services/interfaces";
import { ExternalConfiguration } from "@/models/configData";
import RequestResult from "@/models/requestResult";
import { ResultType } from "@/constants/resulttype";
import MedicationResult from "@/models/medicationResult";
import MedicationStatementHistory from "@/models/medicationStatementHistory";

@injectable()
export class RestMedicationService implements IMedicationService {
    private readonly MEDICATION_STATEMENT_BASE_URI: string =
        "v1/api/MedicationStatement";
    private readonly MEDICATION_BASE_URI: string = "v1/api/Medication";
    private baseUri: string = "";
    private http!: IHttpDelegate;
    private isEnabled: boolean = false;
    private readonly FETCH_ERROR = "Fetch error:";

    constructor() {}

    public initialize(
        config: ExternalConfiguration,
        http: IHttpDelegate
    ): void {
        this.baseUri = config.serviceEndpoints["Medication"];
        this.http = http;
        this.isEnabled = config.webClient.modules["Medication"];
    }

    public getPatientMedicationStatementHistory(
        hdid: string,
        protectiveWord?: string
    ): Promise<RequestResult<MedicationStatementHistory[]>> {
        const headers: Dictionary<string> = {};
        if (protectiveWord) {
            headers["protectiveWord"] = protectiveWord;
        }
        return new Promise((resolve, reject) => {
            if (!this.isEnabled) {
                resolve({
                    pageIndex: 0,
                    pageSize: 0,
                    resourcePayload: [],
                    resultMessage: "",
                    resultStatus: ResultType.Success,
                    totalResultCount: 0,
                });
                return;
            }
            this.http
                .getWithCors<RequestResult<MedicationStatementHistory[]>>(
                    `${this.baseUri}${this.MEDICATION_STATEMENT_BASE_URI}/${hdid}`,
                    headers
                )
                .then((requestResult) => {
                    resolve(requestResult);
                })
                .catch((err) => {
                    console.log(this.FETCH_ERROR + err.toString());
                    reject(err);
                });
        });
    }

    public getMedicationInformation(
        drugIdentifier: string
    ): Promise<MedicationResult> {
        return new Promise((resolve, reject) => {
            return this.http
                .getWithCors<RequestResult<MedicationResult>>(
                    `${this.baseUri}${this.MEDICATION_BASE_URI}/${drugIdentifier}`
                )
                .then((requestResult) => {
                    this.handleResult(requestResult, resolve, reject);
                })
                .catch((err) => {
                    console.log(this.FETCH_ERROR + err.toString());
                    reject(err);
                });
        });
    }

    private handleResult(
        requestResult: RequestResult<any>,
        resolve: any,
        reject: any
    ) {
        if (requestResult.resultStatus === ResultType.Success) {
            resolve(requestResult.resourcePayload);
        } else {
            reject(requestResult.resultMessage);
        }
    }
}
