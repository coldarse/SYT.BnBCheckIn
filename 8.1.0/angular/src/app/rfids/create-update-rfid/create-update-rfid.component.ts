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
  rfid?: RFIDDto = {} as RFIDDto;

  units: any[];

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
  }

  selectUnit(event: any){
    this.rfid.unitId = event.target.value;
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
        }
      );
    }
    else{
      this._rfidService.create(this.rfid).subscribe(
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
