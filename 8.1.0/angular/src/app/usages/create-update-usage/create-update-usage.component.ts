import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { UsageDto } from '../../../shared/service-proxies/usages/model';
import { UsageService } from '../../../shared/service-proxies/usages/usage.service';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-create-update-usage',
  templateUrl: './create-update-usage.component.html',
  styleUrls: ['./create-update-usage.component.css']
})
export class CreateUpdateUsageComponent extends AppComponentBase
 implements OnInit {

  saving = false;
  isCreate = true;
  usage?: UsageDto = {} as UsageDto;

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _usageService: UsageService,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if(this.usage.id != undefined){
      this.isCreate = false;
    }
  }

  save(): void {
    this.saving = true;

    if(this.usage.id != undefined){
      this._usageService.update(this.usage).subscribe(
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
      this._usageService.create(this.usage).subscribe(
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
