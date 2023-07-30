import { Component, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { UsageDto } from '@shared/service-proxies/usages/model'
import { UsageService } from '@shared/service-proxies/usages/usage.service'
import { CreateUpdateUsageComponent } from '../usages/create-update-usage/create-update-usage.component'

class PagedUsagesRequestDto extends PagedRequestDto{
  keyword: string
}

@Component({
  selector: 'app-usages',
  templateUrl: './usages.component.html',
  styleUrls: ['./usages.component.css']
})
export class UsagesComponent extends PagedListingComponentBase<UsageDto> {

  keyword = '';
  usages: any[] = [];

  constructor(
    injector: Injector,
    private _usageService: UsageService,
    private _modalService: BsModalService
  ){
    super(injector);
  }

  createUsage(){
    this.showCreateOrEditUsageDialog();
  }

  editUsage(entity: UsageDto){
    this.showCreateOrEditUsageDialog(entity);
  }

  private showCreateOrEditUsageDialog(entity?: UsageDto){
    let createOrEditUsageDialog: BsModalRef;
    if(!entity){
      createOrEditUsageDialog = this._modalService.show(
        CreateUpdateUsageComponent,
        {
          class: 'modal-lg',
        }
      );
    }
    else{
      createOrEditUsageDialog = this._modalService.show(
        CreateUpdateUsageComponent,
        {
          class: 'modal-lg',
          initialState: {
            usage: entity
          },
        }
      );
    }

    createOrEditUsageDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  clearFilters(): void {
    this.keyword = '';
    this.getDataPage(1);
  }

  protected delete(entity: UsageDto): void{
    abp.message.confirm(
      '',
      undefined,
      (result: boolean) => {
        if (result) {
          this._usageService.delete(entity.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  isButtonVisible(action: string): boolean {
    return this.permission.isGranted('Pages.Usage.' + action);
  }

  protected list(
    request: PagedUsagesRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    this._usageService
    .getAll(
      request
    ).pipe(
      finalize(() => {
        finishedCallback();
      })
    )
    .subscribe((result: any) => {
      this.usages = [];
        result.result.items.forEach((element: UsageDto) => {

          let tempUsage = {
            id: element.id,
            unit: element.unit,
            pico: element.pico,
            rFID: element.rFID,
            building: element.building,
            startTime: element.startTime,
            endTime: element.endTime,
            checkInRef: element.checkInRef,
          }

          this.usages.push(tempUsage);
        });
      this.showPaging(result, pageNumber);
    });
  }
}
