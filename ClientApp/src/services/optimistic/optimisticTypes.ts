import { BaseDto } from "@/services/openapi/types";

export interface OptimisticDto extends BaseDto {
    isManuallySet?: boolean;
    isDeleted?: boolean;
    isPending?: boolean;
    serverError?: string;
}

export type WithOptimistic<T extends BaseDto> = T & OptimisticDto;
