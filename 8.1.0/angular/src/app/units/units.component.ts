import { Component, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { UnitDto } from '@shared/service-proxies/units/model'
import { UnitService } from '@shared/service-proxies/units/unit.service'
import { CreateUpdateUnitComponent } from '../units/create-update-unit/create-update-unit.component'
import { BuildingService } from '@shared/service-proxies/buildings/building.service';

class PagedUnitsRequestDto extends PagedRequestDto{
  keyword: string
}

@Component({
  selector: 'app-units',
  templateUrl: './units.component.html',
  styleUrls: ['./units.component.css']
})
export class UnitsComponent extends PagedListingComponentBase<UnitDto> {

  keyword = '';
  units: any[] = [];
  buildings: any[] = [];

  constructor(
    injector: Injector,
    private _unitService: UnitService,
    private _buildingService: BuildingService,
    private _modalService: BsModalService
  ){
    super(injector);
  }

  createUnit(){
    this.showCreateOrEditUnitDialog();
  }

  editUnit(entity: UnitDto){
    this.showCreateOrEditUnitDialog(entity);
  }

  private showCreateOrEditUnitDialog(entity?: UnitDto){
    let createOrEditUnitDialog: BsModalRef;
    if(!entity){
      createOrEditUnitDialog = this._modalService.show(
        CreateUpdateUnitComponent,
        {
          class: 'modal-lg',
          initialState: {
            buildings: this.buildings,
          }
        }
      );
    }
    else{
      createOrEditUnitDialog = this._modalService.show(
        CreateUpdateUnitComponent,
        {
          class: 'modal-lg',
          initialState: {
            unit: entity,
            buildings: this.buildings,
          },
        }
      );
    }

    createOrEditUnitDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  clearFilters(): void {
    this.keyword = '';
    this.getDataPage(1);
  }

  protected delete(entity: UnitDto): void{
    abp.message.confirm(
      '',
      undefined,
      (result: boolean) => {
        if (result) {
          this._unitService.delete(entity.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  isButtonVisible(action: string): boolean {
    return this.permission.isGranted('Pages.Unit.' + action);
  }

  protected list(
    request: PagedUnitsRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    this._unitService
    .getAll(
      request
    ).pipe(
      finalize(() => {
        finishedCallback();
      })
    )
    .subscribe((result: any) => {
      this.units = [];
      this._buildingService.getAllBuildings().subscribe((results: any) => {
        this.buildings = results.result;
        result.result.items.forEach((element: UnitDto) => {
          let building = results.result.find(x => x.id == element.buildingId);

          let tempUnit = {
            id: element.id,
            building: building.name,
            buildingId: element.buildingId,
            unitNo: element.unitNo,
            status: element.status,
            remark: element.remark,
          }

          this.units.push(tempUnit);
        });
        this.showPaging(result.result, pageNumber);
      });
    });
  }
}
