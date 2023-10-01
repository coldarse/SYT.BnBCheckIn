import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { RFIDDto } from '../../../shared/service-proxies/rfids/model';
import { RFIDService } from '../../../shared/service-proxies/rfids/rfid.service';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-create-update-rfid',
  templateUrl: './create-update-rfid.component.html',
  styleUrls: ['./create-update-rfid.component.css']
})
export class CreateUpdateRFIDComponent extends AppComponentBase
 implements OnInit {

  saving = false;
  isCreate = true;
  isExists = false;
  rfid?: RFIDDto = {} as RFIDDto;

  type = 0;

  units: any[];

  tempUnits = '';

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _rfidService: RFIDService,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if(this.rfid.id != undefined){
      this.isCreate = false;
    }

    this.tempUnits = JSON.stringify(this.units);
  }

  selectUnit(event: any){
    this.rfid.unitId = event.target.value;
  }

  selectType(event: any){
    this.type = event.target.value;
    if(this.type == 1){
      this.units = JSON.parse(this.tempUnits).filter(x => !x.unitNo.toLowerCase().includes('master'))
    }
    else if(this.type == 2){
      this.units = JSON.parse(this.tempUnits).filter(x => x.unitNo.toLowerCase().includes('master'))
    }
    else{
      this.units = JSON.parse(this.tempUnits);
    }
  }

  save(): void {
    this.saving = true;

    if(this.rfid.id != undefined){
      this._rfidService.update(this.rfid).subscribe(
        () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        () => {
          this.saving = false;
          this.isExists = false;
        }
      );
    }
    else{
      this._rfidService.isExist(this.rfid.value).subscribe((data: any) => {
        if(data.result == true){
          this._rfidService.create(this.rfid).subscribe(
            () => {
              this.notify.info(this.l('SavedSuccessfully'));
              this.bsModalRef.hide();
              this.onSave.emit();
            },
            () => {
              this.saving = false;
              this.isExists = false;
            }
          );
        }
        else{
          this.rfid = {} as RFIDDto;
          this.saving = false;
          this.isExists = true;
        }
      });
    }

  }

}
