import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { BuildingDto } from '../../../shared/service-proxies/buildings/model';
import { BuildingService } from '../../../shared/service-proxies/buildings/building.service';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-create-update-building',
  templateUrl: './create-update-building.component.html',
  styleUrls: ['./create-update-building.component.css']
})
export class CreateUpdateBuildingComponent extends AppComponentBase
 implements OnInit {

  saving = false;
  isCreate = true;
  building?: BuildingDto = {} as BuildingDto;

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _buildingService: BuildingService,
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
      this._buildingService.update(this.building).subscribe(
        () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        () => {
          this.saving = false;
        }
      );
    }
    else{
      this._buildingService.create(this.building).subscribe(
        () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        () => {
          this.saving = false;
        }
      );
    }

  }

}
