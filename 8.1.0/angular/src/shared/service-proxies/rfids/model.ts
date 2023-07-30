import type { PagedAndSortedResultRequestDto, EntityDto } from '@abp/ng.core';


export interface RFIDDto extends EntityDto<number> {
    value: string;
    unitId: string;
}

export interface PagedRFIDResultRequestDto extends PagedAndSortedResultRequestDto {
    keyword?: string;
    value?: string;
    unitId?: string;
}
