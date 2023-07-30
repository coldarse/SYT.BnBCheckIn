import type { PagedAndSortedResultRequestDto, EntityDto } from '@abp/ng.core';


export interface BuildingDto extends EntityDto<number> {
    name: string;
    address: string;
    city: string;
    state: string;
    postcode: string;
    remark: string;
}

export interface PagedBuildingResultRequestDto extends PagedAndSortedResultRequestDto {
    keyword?: string;
    name?: string;
    address?: string;
    city?: string;
    state?: string;
    postcode?: string;
    remark?: string;
}
