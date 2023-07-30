import type { PagedAndSortedResultRequestDto, EntityDto } from '@abp/ng.core';


export interface UnitDto extends EntityDto<number> {
    buildingId: string;
    unitNo: string;
    status: string;
    remark: string;
}

export interface PagedUnitResultRequestDto extends PagedAndSortedResultRequestDto {
    keyword?: string;
    buildingId?: string;
    unitNo?: string;
    status?: string;
    remark?: string;
}
