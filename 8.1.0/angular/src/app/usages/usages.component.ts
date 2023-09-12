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
  view: any[] = [800, 400];
  

  usage: any[] = [];

  // options
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = false;
  showXAxisLabel = true;
  xAxisLabel = 'Date';
  showYAxisLabel = true;
  yAxisLabel = 'Hours';

  colorScheme = {
    domain: ['#116cb7', '#AAAAAA']
  };

  single: any[] = [
    {
      "name": "18th Aug",
      "value": 5
    },
    {
      "name": "19th Aug",
      "value": 8
    },
    {
      "name": "20th Aug",
      "value": 3
    },
    {
      "name": "21st Aug",
      "value": 7
    },
    {
      "name": "22nd Aug",
      "value": 18
    },
    {
      "name": "23rd Aug",
      "value": 17
    },
    {
      "name": "24th Aug",
      "value": 10
    },
    {
      "name": "25th Aug",
      "value": 4
    },
    {
      "name": "26th Aug",
      "value": 0
    },
    {
      "name": "27th Aug",
      "value": 2
    },
    {
      "name": "28th Aug",
      "value": 5
    },
    {
      "name": "29th Aug",
      "value": 3
    },
    {
      "name": "30th Aug",
      "value": 14
    },
    {
      "name": "31st Aug",
      "value": 9
    },
  ];

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
            rFID: element.rfid,
            building: element.building,
            startTime: element.startTime,
            endTime: element.endTime,
            checkInRef: element.checkInRef,
          }

          this.usages.push(tempUsage);
        });
        this._usageService
        .getDayUsage().pipe(
          finalize(() => {
            finishedCallback();
          })
        ).subscribe((result: any) => {
          this.usage = result.result;
          this.single = result.result[0].usages;
        });
      this.showPaging(result.result, pageNumber);
    });
    
  }

  selected(event: any){
    let index = this.usage.findIndex(x => x.unit == event.target.value);
    this.single = this.usage[index].usages;
  }
}
