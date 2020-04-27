import { injectable } from "inversify";
import { IHttpDelegate, IUserCommentService } from "@/services/interfaces";
import RequestResult from "@/models/requestResult";
import UserComment from "@/models/userComment";
import { ResultType } from "@/constants/resulttype";
import { ExternalConfiguration } from "@/models/configData";

@injectable()
export class RestUserCommentService implements IUserCommentService {
  NOT_IMPLENTED: string = "Method not implemented.";
  private readonly USER_COMMENT_BASE_URI: string = "v1/api/Comment";
  private http!: IHttpDelegate;
  private isEnabled: boolean = false;

  public initialize(config: ExternalConfiguration, http: IHttpDelegate): void {
    this.http = http;
    this.isEnabled = config.externalClient.modules["Comment"];
  }

  public getCommentsForEntry(parentEntryId: string): Promise<RequestResult<UserComment[]>> {
    return new Promise((resolve, reject) => {
      if (!this.isEnabled) {
        resolve({
          pageIndex: 0,
          pageSize: 0,
          resourcePayload: [],
          resultMessage: "",
          resultStatus: ResultType.Success,
          totalResultCount: 0
        });
        return;
      }
      this.http
        .getWithCors<RequestResult<UserComment[]>>(`${this.USER_COMMENT_BASE_URI}?parentEntryId="${parentEntryId}"`, )
        .then(userComments => {
          return resolve(userComments);
        })
        .catch(err => {
          console.log(err);
          return reject(err);
        });
    });
  }

  public createComment(comment: UserComment): Promise<UserComment> {
    return new Promise((resolve, reject) => {
      this.http
        .post<RequestResult<UserComment>>(
          `${this.USER_COMMENT_BASE_URI}/`,
          comment
        )
        .then(result => {
          return this.handleResult(result, resolve, reject);
        })
        .catch(err => {
          console.log(err);
          return reject(err);
        });
    });
  }

  public updateComment(comment: UserComment): Promise<UserComment> {
    return new Promise((resolve, reject) => {
      this.http
        .put<RequestResult<UserComment>>(
          `${this.USER_COMMENT_BASE_URI}/`,
          comment
        )
        .then(result => {
          return this.handleResult(result, resolve, reject);
        })
        .catch(err => {
          console.log(err);
          return reject(err);
        });
    });
  }

  public deleteComment(comment: UserComment): Promise<void> {
    return new Promise((resolve, reject) => {
      this.http
        .delete<RequestResult<UserComment>>(
          `${this.USER_COMMENT_BASE_URI}/`,
          comment
        )
        .then(result => {
          return this.handleResult(result, resolve, reject);
        })
        .catch(err => {
          console.log(err);
          return reject(err);
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
