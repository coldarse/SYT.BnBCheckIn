import type { PagedAndSortedResultRequestDto, EntityDto } from '@abp/ng.core';


export interface UsageDto extends EntityDto<number> {
    unit: string;
    pico: string;
    rfid: string;
    building: string;
    startTime: string;
    endTime: string;
    checkInRef: string;
}

export interface PagedUsageResultRequestDto extends PagedAndSortedResultRequestDto {
    keyword?: string;
    unit?: string;
    pico?: string;
    rfid?: string;
    building?: string;
    startTime?: string;
    endTime?: string;
    checkInRef?: string;
}
