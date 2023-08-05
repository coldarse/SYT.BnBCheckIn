import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { PicoDto } from '../../../shared/service-proxies/picos/model';
import { PicoService } from '../../../shared/service-proxies/picos/pico.service';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-create-update-pico',
  templateUrl: './create-update-pico.component.html',
  styleUrls: ['./create-update-pico.component.css']
})
export class CreateUpdatePicoComponent extends AppComponentBase
 implements OnInit {

  saving = false;
  isCreate = true;
  pico?: PicoDto = {} as PicoDto;

  units: any[];

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _picoService: PicoService,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if(this.pico.id != undefined){
      this.isCreate = false;
    }
  }

  selectUnit(event: any){
    this.pico.unitId = event.target.value;
  }

  save(): void {
    this.saving = true;

    if(this.pico.id != undefined){
      this._picoService.update(this.pico).subscribe(
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
      this._picoService.create(this.pico).subscribe(
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
