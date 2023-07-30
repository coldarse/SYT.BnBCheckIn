import { Component, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { BuildingDto } from '@shared/service-proxies/buildings/model'
import { BuildingService } from '@shared/service-proxies/buildings/building.service'
import { CreateUpdateBuildingComponent } from '../buildings/create-update-building/create-update-building.component'

class PagedBuildingsRequestDto extends PagedRequestDto{
  keyword: string
}

@Component({
  selector: 'app-buildings',
  templateUrl: './buildings.component.html',
  styleUrls: ['./buildings.component.css']
})
export class BuildingsComponent extends PagedListingComponentBase<BuildingDto> {

  keyword = '';
  buildings: any[] = [];

  constructor(
    injector: Injector,
    private _buildingService: BuildingService,
    private _modalService: BsModalService
  ){
    super(injector);
  }

  createBuilding(){
    this.showCreateOrEditBuildingDialog();
  }

  editBuilding(entity: BuildingDto){
    this.showCreateOrEditBuildingDialog(entity);
  }

  private showCreateOrEditBuildingDialog(entity?: BuildingDto){
    let createOrEditBuildingDialog: BsModalRef;
    if(!entity){
      createOrEditBuildingDialog = this._modalService.show(
        CreateUpdateBuildingComponent,
        {
          class: 'modal-lg',
        }
      );
    }
    else{
      createOrEditBuildingDialog = this._modalService.show(
        CreateUpdateBuildingComponent,
        {
          class: 'modal-lg',
          initialState: {
            building: entity
          },
        }
      );
    }

    createOrEditBuildingDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  clearFilters(): void {
    this.keyword = '';
    this.getDataPage(1);
  }

  protected delete(entity: BuildingDto): void{
    abp.message.confirm(
      '',
      undefined,
      (result: boolean) => {
        if (result) {
          this._buildingService.delete(entity.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  isButtonVisible(action: string): boolean {
    return this.permission.isGranted('Pages.Building.' + action);
  }

  protected list(
    request: PagedBuildingsRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    this._buildingService
    .getAll(
      request
    ).pipe(
      finalize(() => {
        finishedCallback();
      })
    )
    .subscribe((result: any) => {
      this.buildings = [];
        result.result.items.forEach((element: BuildingDto) => {

          let tempBuilding = {
            id: element.id,
            name: element.name,
            address: element.address,
            city: element.city,
            state: element.state,
            postcode: element.postcode,
            remark: element.remark,
          }

          this.buildings.push(tempBuilding);
        });
      this.showPaging(result, pageNumber);
    });
  }
}
