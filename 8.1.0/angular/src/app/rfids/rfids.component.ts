import { Component, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { RFIDDto } from '@shared/service-proxies/rfids/model'
import { RFIDService } from '@shared/service-proxies/rfids/rfid.service'
import { CreateUpdateRFIDComponent } from '../rfids/create-update-rfid/create-update-rfid.component'
import { UnitService } from '@shared/service-proxies/units/unit.service';

class PagedRFIDSRequestDto extends PagedRequestDto{
  keyword: string
}

@Component({
  selector: 'app-rfids',
  templateUrl: './rfids.component.html',
  styleUrls: ['./rfids.component.css']
})
export class RFIDSComponent extends PagedListingComponentBase<RFIDDto> {

  keyword = '';
  rfids: any[] = [];

  units: any[] = [];

  constructor(
    injector: Injector,
    private _rfidService: RFIDService,
    private _unitService: UnitService,
    private _modalService: BsModalService
  ){
    super(injector);
  }

  createRFID(){
    this.showCreateOrEditRFIDDialog();
  }

  editRFID(entity: RFIDDto){
    this.showCreateOrEditRFIDDialog(entity);
  }

  private showCreateOrEditRFIDDialog(entity?: RFIDDto){
    let createOrEditRFIDDialog: BsModalRef;
    if(!entity){
      createOrEditRFIDDialog = this._modalService.show(
        CreateUpdateRFIDComponent,
        {
          class: 'modal-lg',
          initialState: {
            units: this.units,
          },
        }
      );
    }
    else{
      createOrEditRFIDDialog = this._modalService.show(
        CreateUpdateRFIDComponent,
        {
          class: 'modal-lg',
          initialState: {
            rfid: entity,
            units: this.units,
          },
        }
      );
    }

    createOrEditRFIDDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  clearFilters(): void {
    this.keyword = '';
    this.getDataPage(1);
  }

  protected delete(entity: RFIDDto): void{
    abp.message.confirm(
      '',
      undefined,
      (result: boolean) => {
        if (result) {
          this._rfidService.delete(entity.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  isButtonVisible(action: string): boolean {
    return this.permission.isGranted('Pages.RFID.' + action);
  }

  protected list(
    request: PagedRFIDSRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    this._rfidService
    .getAll(
      request
    ).pipe(
      finalize(() => {
        finishedCallback();
      })
    )
    .subscribe((result: any) => {
      this.rfids = [];
      this._unitService.getAllUnits().subscribe((results: any) => {
        this.units = results.result;
        result.result.items.forEach((element: RFIDDto) => {
          let unit = results.result.find(x => x.id == element.unitId);

          if (unit == undefined) return;
          
          let tempRFID = {
            id: element.id,
            unit: unit.unitNo,
            value: element.value,
            unitId: element.unitId,
          }

          this.rfids.push(tempRFID);
        });
      });
      this.showPaging(result.result, pageNumber);
    });
  }
}
