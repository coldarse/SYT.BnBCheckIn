import { Component, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { PicoDto } from '@shared/service-proxies/picos/model'
import { PicoService } from '@shared/service-proxies/picos/pico.service'
import { CreateUpdatePicoComponent } from '../picos/create-update-pico/create-update-pico.component'
import { UnitService } from '@shared/service-proxies/units/unit.service';

class PagedPicosRequestDto extends PagedRequestDto{
  keyword: string
}

@Component({
  selector: 'app-picos',
  templateUrl: './picos.component.html',
  styleUrls: ['./picos.component.css']
})
export class PicosComponent extends PagedListingComponentBase<PicoDto> {

  keyword = '';
  picos: any[] = [];

  units: any[] = [];
  unassignedunits: any[] = [];

  constructor(
    injector: Injector,
    private _picoService: PicoService,
    private _unitService: UnitService,
    private _modalService: BsModalService
  ){
    super(injector);
  }

  createPico(){
    this.showCreateOrEditPicoDialog();
  }

  editPico(entity: PicoDto){
    this.showCreateOrEditPicoDialog(entity);
  }

  private showCreateOrEditPicoDialog(entity?: PicoDto){
    let createOrEditPicoDialog: BsModalRef;
    if(!entity){
      createOrEditPicoDialog = this._modalService.show(
        CreateUpdatePicoComponent,
        {
          class: 'modal-lg',
          initialState: {
            units: this.unassignedunits,
          },
        }
      );
    }
    else{
      createOrEditPicoDialog = this._modalService.show(
        CreateUpdatePicoComponent,
        {
          class: 'modal-lg',
          initialState: {
            pico: entity,
            units: this.unassignedunits,
          },
        }
      );
    }

    createOrEditPicoDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  clearFilters(): void {
    this.keyword = '';
    this.getDataPage(1);
  }

  protected delete(entity: PicoDto): void{
    abp.message.confirm(
      '',
      undefined,
      (result: boolean) => {
        if (result) {
          this._picoService.delete(entity.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  isButtonVisible(action: string): boolean {
    return this.permission.isGranted('Pages.Pico.' + action);
  }

  protected list(
    request: PagedPicosRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    this._picoService
    .getAll(
      request
    ).pipe(
      finalize(() => {
        finishedCallback();
      })
    )
    .subscribe((result: any) => {
      this.picos = [];
      this._unitService.getAllUnits().subscribe((results: any) => {
        this.units = results.result.filter((obj) => {return !obj.unitNo.includes('Master')});
        result.result.items.forEach((element: PicoDto) => {
          let unit = results.result.find(x => x.id == element.unitId);

          let tempPico = {
            id: element.id,
            unit: unit.unitNo,
            unitId: element.unitId,
            name: element.name,
          }

          this.picos.push(tempPico);
        });

        this._unitService.getAllUnassignedUnits().subscribe((_results: any) => {
          this.unassignedunits = _results.result.filter((obj) => {return !obj.unitNo.includes('Master')});

          this.showPaging(result.result, pageNumber);
        })
      });

      
    });
  }
}
