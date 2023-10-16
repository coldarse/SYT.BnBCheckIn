import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { BuildingDto } from '../../../shared/service-proxies/buildings/model';
import { BuildingService } from '../../../shared/service-proxies/buildings/building.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { UnitService } from '@shared/service-proxies/units/unit.service';
import { RFIDService } from '@shared/service-proxies/rfids/rfid.service';

@Component({
  selector: 'app-select-building',
  templateUrl: './select-building.component.html',
  styleUrls: ['./select-building.component.css']
})
export class SelectBuildingComponent extends AppComponentBase
 implements OnInit {

  saving = false;
  building?: BuildingDto = {
    remark: '-'
  } as BuildingDto;

  buildingId: number;
  selectedBuildingId: number = 0;
  buildingName: string;
  toDeleteId: number;

  buildings: any[];

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _buildingService: BuildingService,
    public _unitService: UnitService,
    public _rfidService: RFIDService,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
  }

  selectBuilding(event: any){
    this.selectedBuildingId = event.target.value;
  }

  save(): void {
    this.saving = true;

    let body = {
        buildingId: this.toDeleteId,
        newBuildingId: this.selectedBuildingId
    }

    let data = {
        buildingId: this.selectedBuildingId,
        currentBuildingId: this.toDeleteId
    }

    this._rfidService.updateNewMaster(data).subscribe((elem: any) => {
        if(elem.result){
            this._unitService.updateNewBuilding(body).subscribe((data: any) => {
                if(data.result){
                    this._buildingService.delete(this.toDeleteId).subscribe(() => {
                        this._unitService.deleteMasterUnit(this.buildingName).subscribe(() => {
                            this.notify.info(this.l('SuccessfullyDeleted'));
                            this.bsModalRef.hide();
                            this.onSave.emit();
                        },
                        () => {
                        this.saving = false;
                        });
                    });
                }
            });
        }
    });
  }

}
