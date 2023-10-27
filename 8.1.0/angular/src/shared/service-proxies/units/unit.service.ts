import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { catchError, retry, throwError } from 'rxjs';
import { PagedUnitResultRequestDto, UnitDto } from './model';
import { AppConsts } from '@shared/AppConsts';

@Injectable()
export class UnitService {

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

    //Create Unit
    create(body: UnitDto){
        return this.http.post(
            this.url + '/api/services/app/Unit/Create',
            body,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    //Update Unit
    update(body: UnitDto){
        return this.http.put(
            this.url + '/api/services/app/Unit/Update',
            body,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    //Delete Unit
    delete(id: number){
        return this.http.delete(
            this.url + `/api/services/app/Unit/Delete?Id=${id.toString()}`,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    //Get Unit
    get(id: number){
        return this.http.get(
            this.url + `/api/services/app/Unit/Get?Id=${id}`,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    //Get All Units
    getAll(body: PagedUnitResultRequestDto){
        let url_ = this.url + "/api/services/app/Unit/GetAll?";

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

    //GetAllUnits
    getAllUnits(){
        let url_ = this.url + "/api/services/app/Unit/GetAllUnits";

        return this.http.get(
            url_,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )

    }

    // Update Master Unit
    updateMasterUnitName(body: any){
        let url_ = this.url + "/api/services/app/Unit/updateMasterUnitName";

        return this.http.put(
            url_,
            body,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    // Delete Master Unit
    deleteMasterUnit(name: string){
        let url_ = this.url + `/api/services/app/Unit/deleteMasterUnit?input=${name}`;

        return this.http.delete(
            url_,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    // Get Assigned Units
    getAreThereAssignedUnits(id: number){
        let url_ = this.url + `/api/services/app/Unit/getAreThereAssignedUnits?buildingId=${id}`;

        return this.http.get(
            url_,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    // Update New Building Ids
    updateNewBuilding(body: any){
        let url_ = this.url + "/api/services/app/Unit/updateNewBuilding";

        return this.http.put(
            url_,
            body,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

    // Get Unit Status Counts
    getStatusCounts(){
        let url_ = this.url + `/api/services/app/Unit/GetStatusCount`;

        return this.http.get(
            url_,
            this.options_
        ).pipe(
            retry(1),
            catchError(this.handleError),
        )
    }

}
