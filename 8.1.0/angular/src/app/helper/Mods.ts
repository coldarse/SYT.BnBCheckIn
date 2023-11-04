import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
  })

export class Mods {

    static CompareUnitNo(a, b) {
      if ( a.unitNo < b.unitNo ){
        return -1;
      }
      if ( a.unitNo > b.unitNo ){
        return 1;
      }
      return 0;
    }
     
    static CompareName(a, b) {
      if ( a.name < b.name ){
        return -1;
      }
      if ( a.name > b.name ){
        return 1;
      }
      return 0;
    }

    static Compare(a, b) {
      if ( a < b ){
        return -1;
      }
      if ( a > b ){
        return 1;
      }
      return 0;
    }
}
