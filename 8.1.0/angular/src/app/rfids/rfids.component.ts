import { Component, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { RFIDDto } from '@shared/service-proxies/rfids/model'
import { RFIDService } from '@shared/service-proxies/rfids/rfid.service'
import { CreateUpdateRFIDComponent } from '../rfids/create-update-rfid/create-update-rfid.component'

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

  constructor(
    injector: Injector,
    private _rfidService: RFIDService,
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
        }
      );
    }
    else{
      createOrEditRFIDDialog = this._modalService.show(
        CreateUpdateRFIDComponent,
        {
          class: 'modal-lg',
          initialState: {
            rfid: entity
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
        result.result.items.forEach((element: RFIDDto) => {

          let tempRFID = {
            id: element.id,
            value: element.value,
            unitId: element.unitId,
          }

          this.rfids.push(tempRFID);
        });
      this.showPaging(result, pageNumber);
    });
  }
}
