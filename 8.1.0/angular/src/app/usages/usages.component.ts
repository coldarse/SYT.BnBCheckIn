import { Component, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { UsageDto } from '@shared/service-proxies/usages/model'
import { UsageService } from '@shared/service-proxies/usages/usage.service'
import { CreateUpdateUsageComponent } from '../usages/create-update-usage/create-update-usage.component'
import * as XLSX from 'xlsx';
import * as moment from 'moment'; 

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

  EXCEL_EXTENSION = '.xlsx';
  filename = 'Usage';
  days = 7;

  usage: string = '';
  forExcel: string = '';
  unitForExcel: string = ''

  // options
  legend: boolean = true;
  showLabels: boolean = true;
  animations: boolean = true;
  xAxis: boolean = true;
  yAxis: boolean = true;
  showYAxisLabel: boolean = true;
  showXAxisLabel: boolean = true;
  xAxisLabel: string = 'Date';
  yAxisLabel: string = 'Hours';
  timeline: boolean = true;

  colorScheme = {
    domain: ['#5AA454', '#E44D25', '#CFC0BB', '#7aa3e5', '#a8385d', '#aae3f5']
  };

  noOfDays = [
    {
      value: 7,
      day: '7 days'
    },
    {
      value: 14,
      day: '14 days'
    },
    {
      value: 30,
      day: '30 days'
    },
    {
      value: 60,
      day: '60 days'
    },
    {
      value: 90,
      day: '90 days'
    },
  ];

  single: any[] = [];
  buildings: any[] = [];
  units: any[] = [];
  arrayForExcel: any[] = [];

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
    // this._usageService
    // .getAll(
    //   request
    // ).pipe(
    //   finalize(() => {
    //     finishedCallback();
    //   })
    // )
    // .subscribe((result: any) => {
    //   this.usages = [];
    //     result.result.items.forEach((element: UsageDto) => {

    //       let tempUsage = {
    //         id: element.id,
    //         unit: element.unit,
    //         pico: element.pico,
    //         rFID: element.rfid,
    //         building: element.building,
    //         startTime: element.startTime,
    //         endTime: element.endTime,
    //         checkInRef: element.checkInRef,
    //       }

    //       this.usages.push(tempUsage);
    //     });
    this._usageService
    .getDayUsage(7).pipe(
      finalize(() => {
        finishedCallback();
      })
    ).subscribe((result: any) => {
      this.buildings = [...new Set(result.result.nested.map(item => item.building))];
      this.buildings.unshift('All');
      this.usage = JSON.stringify(result.result.nested);
      this.single = result.result.nested;
      this.forExcel = result.result.notNested;
    });
      // this.showPaging(result.result, pageNumber);
    // });
    
  }

  exportexcel(){
    const date = moment(new Date(), "DD-MM-YYYY");
    let fileName = '';
    if(this.unitForExcel != ''){
      let tempArr: any[] = JSON.parse(this.forExcel);
      this.arrayForExcel = tempArr.filter((obj: any) => {
        return obj.unit === this.unitForExcel;
      });
      fileName = `${this.unitForExcel}_${this.filename}_${this.days}_days_${date.format("YYYY-MM-DD")}${this.EXCEL_EXTENSION}`
    }
    else{
      this.arrayForExcel = JSON.parse(this.forExcel);
      `${this.filename}_${this.days}_days_${date.format("YYYY-MM-DD")}${this.EXCEL_EXTENSION}`
    }

    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.arrayForExcel);
    const workbook: XLSX.WorkBook = XLSX.utils.book_new(); 
    console.log(ws)
    // save to file
    XLSX.utils.book_append_sheet(workbook, ws, 'Sheet1');
    XLSX.writeFile(workbook, fileName);
  }

  selectedBuilding(event: any){
    if(event.target.value == "All"){
      this.single = JSON.parse(this.usage);
    }
    else{
      let temp_usage: [] = JSON.parse(this.usage);
      this.single = temp_usage.filter((obj: any) => {
        return obj.building === event.target.value;
      });
    }
  }

  selectedUnit(event: any){
    if(event.target.value == "All"){
      this.single = JSON.parse(this.usage);
      this.unitForExcel = '';
    }
    else{
      this.unitForExcel = event.target.value;
      let temp_usage: [] = JSON.parse(this.usage);
      this.single = temp_usage.filter((obj: any) => {
        return obj.name === event.target.value;
      });
    }
  }

  selectedDays(event: any){
    this.days = event.target.value;
    this._usageService
    .getDayUsage(event.target.value)
    .subscribe((result: any) => {
      this.buildings = [...new Set(result.result.nested.map(item => item.building))];
      this.buildings.unshift('All');
      this.units = [...new Set(result.result.nested.map(item => item.name))];
      this.units.unshift('All');
      this.usage = JSON.stringify(result.result.nested);
      this.single = result.result.nested;
      this.forExcel = JSON.stringify(result.result.notNested);
      console.log(this.forExcel)
    });
  }

  capitalizeFirstLetter(str: string){
    return str.charAt(0).toUpperCase()+str.slice(1);
  }
}
