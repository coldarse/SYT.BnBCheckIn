import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { UnitDto } from '../../../shared/service-proxies/units/model';
import { UnitService } from '../../../shared/service-proxies/units/unit.service';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-create-update-unit',
  templateUrl: './create-update-unit.component.html',
  styleUrls: ['./create-update-unit.component.css']
})
export class CreateUpdateUnitComponent extends AppComponentBase
 implements OnInit {

  saving = false;
  isCreate = true;
  unit?: UnitDto = {
    remark: '-'
  } as UnitDto;

  buildings: any[];

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _unitService: UnitService,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if(this.unit.id != undefined){
      this.isCreate = false;
    }
  }

  selectBuilding(event: any){
    this.unit.buildingId = event.target.value;
  }
  
  selectStatus(event: any){
    this.unit.status = event.target.value;
  }

  save(): void {
    this.saving = true;

    if(this.unit.id != undefined){
      this._unitService.update(this.unit).subscribe(
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
      this._unitService.create(this.unit).subscribe(
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
