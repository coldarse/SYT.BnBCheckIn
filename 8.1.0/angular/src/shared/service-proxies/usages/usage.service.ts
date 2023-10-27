import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { catchError, retry, throwError } from 'rxjs';
import { PagedUpdatedUsageResultRequestDto, PagedUsageResultRequestDto, UsageDto } from './model';
import { AppConsts } from '@shared/AppConsts';

@Injectable()
export class UsageService {

    url = '';
    options_: any;

    constructor(private http: HttpClient){
        this.url = AppConsts.remoteServiceBaseUrl;
        this.options_ = {
            headers: new HttpHeaders({
                "Content-Type": "application/json-patch+json",
                "Accept": "text/plain"
            })
        };
    }

    private handleError(error: HttpErrorResponse) {
        if (error.error instanceof ErrorEvent) {
          console.error('An error occurred:', error.error.message);
        }
        else {
          console.error(
            `Backend returned code ${error.status}, ` +
            `body was: ${error.error}`);
        }
        return throwError(() => new Error(error.error.message));
    }

    //Create Usage
    create(body: UsageDto){
        return this.http.post(
            this.url + '/api/services/app/Usage/Create',
            body,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    //Update Usage
    update(body: UsageDto){
        return this.http.put(
            this.url + '/api/services/app/Usage/Update',
            body,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    //Delete Usage
    delete(id: number){
        return this.http.delete(
            this.url + `/api/services/app/Usage/Delete?Id=${id.toString()}`,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    //Get Usage
    get(id: number){
        return this.http.get(
            this.url + `/api/services/app/Usage/Get?Id=${id}`,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    //Get All Usages
    getAll(body: PagedUsageResultRequestDto){
        let url_ = this.url + "/api/services/app/Usage/GetAll?";

        if (body.keyword === null)
            throw new Error("The parameter 'keyword' cannot be null.");
        else if (body.keyword !== undefined)
            url_ += "Keyword=" + encodeURIComponent("" + body.keyword) + "&";

        if (body.skipCount !== undefined)
            url_ += "SkipCount=" + encodeURIComponent("" + body.skipCount) + "&";


        url_ = url_.replace(/[?&]$/, "");


        return this.http.get(
            url_ + `&MaxResultCount=10`,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    // Get All Updated Usages
    getAllUpdatedUsage(body: PagedUpdatedUsageResultRequestDto){
        let url_ = this.url + "/api/services/app/Usage/GetUpdatedAll?";

        if (body.unit === null)
            throw new Error("The parameter 'unit' cannot be null.");
        else if (body.unit !== undefined)
            url_ += "Unit=" + encodeURIComponent("" + body.unit) + "&";

        if (body.startTime === null)
            throw new Error("The parameter 'startTime' cannot be null.");
        else if (body.startTime !== undefined)
            url_ += "StartTime=" + encodeURIComponent("" + body.startTime) + "&";

        if (body.endTime === null)
            throw new Error("The parameter 'endTime' cannot be null.");
        else if (body.endTime !== undefined)
            url_ += "EndTime=" + encodeURIComponent("" + body.endTime) + "&";

        if (body.skipCount !== undefined)
            url_ += "SkipCount=" + encodeURIComponent("" + body.skipCount) + "&";


        url_ = url_.replace(/[?&]$/, "");


        return this.http.get(
            url_ + `&MaxResultCount=10`,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    //Usage Report
    getDayUsage(days: number){
        return this.http.get(
            this.url + `/api/services/app/Usage/GetDayUsage?days=${days}`,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }
    

}
