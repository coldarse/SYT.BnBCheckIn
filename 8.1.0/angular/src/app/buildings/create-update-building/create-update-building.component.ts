import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { BuildingDto } from '../../../shared/service-proxies/buildings/model';
import { BuildingService } from '../../../shared/service-proxies/buildings/building.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { UnitService } from '@shared/service-proxies/units/unit.service';
import { title } from 'process';

@Component({
  selector: 'app-create-update-building',
  templateUrl: './create-update-building.component.html',
  styleUrls: ['./create-update-building.component.css']
})
export class CreateUpdateBuildingComponent extends AppComponentBase
 implements OnInit {

  saving = false;
  isCreate = true;
  building?: BuildingDto = {
    remark: '-'
  } as BuildingDto;

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _buildingService: BuildingService,
    public _unitService: UnitService,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if(this.building.id != undefined){
      this.isCreate = false;
    }
  }

  save(): void {
    this.saving = true;

    if(this.building.id != undefined){
      this._buildingService.update(this.building).subscribe((data: any) => {
        if(data.result.name == 'ERR501'){
          this.notify.error(data.result.remark);
          this.bsModalRef.hide();
          this.onSave.emit();
        }
        else{
          const body = {
            buildingId: this.building.id,
            name: this.building.name
          }
          this._unitService.updateMasterUnitName(body).subscribe(
            () => {
              this.notify.info(this.l('SavedSuccessfully'));
              this.bsModalRef.hide();
              this.onSave.emit();
            },
            () => {
              this.saving = false;
            }
          )
        }
      });
    }
    else{
      this._buildingService.create(this.building).subscribe((data: any) => {
        if(data.result.name == 'ERR501'){
          this.notify.error(data.result.remark);
          this.bsModalRef.hide();
          this.onSave.emit();
        }
        else{
          const unit = {
            buildingId: data.result.id,
            unitNo: data.result.name + ' Master',
            status: 'Vacant',
            remark: '-'
          }
          this._unitService.create(unit).subscribe(
            () => {
              this.notify.info(this.l('SavedSuccessfully'));
              this.bsModalRef.hide();
              this.onSave.emit();
            },
            () => {
              this.saving = false;
            }
          )
        }
      });
    }

  }

}
