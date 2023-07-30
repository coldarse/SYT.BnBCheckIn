import type { PagedAndSortedResultRequestDto, EntityDto } from '@abp/ng.core';


export interface PicoDto extends EntityDto<number> {
    name: string;
    unitId: string;
}

export interface PagedPicoResultRequestDto extends PagedAndSortedResultRequestDto {
    keyword?: string;
    name?: string;
    unitId?: string;
}
