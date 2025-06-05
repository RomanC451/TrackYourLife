import { BaseDto } from "@/services/openapi/types";
import { OptimisticDto, WithOptimistic } from "./optimisticTypes";

export function markAsManuallySet<T extends BaseDto>(data: T): WithOptimistic<T> {
    return {
        ...data,
        isManuallySet: true,
        isPending: true
    };
}

export function markAsFromApi<T extends BaseDto>(data: T): WithOptimistic<T> {
    return {
        ...data,
        isManuallySet: false,
        isPending: false
    };
}

export function markAsDeleted<T extends BaseDto>(data: WithOptimistic<T>): WithOptimistic<T> {
    return {
        ...data,
        isDeleted: true,
        isPending: true
    };
}

export function markWithError<T extends BaseDto>(data: WithOptimistic<T>, error: string): WithOptimistic<T> {
    return {
        ...data,
        serverError: error,
        isPending: false
    };
}

export function isOptimistic<T extends BaseDto>(data: WithOptimistic<T>): boolean {
    return data.isManuallySet === true;
}

export function isPending<T extends BaseDto>(data: WithOptimistic<T>): boolean {
    return data.isPending === true;
}

export function hasError<T extends BaseDto>(data: WithOptimistic<T>): boolean {
    return !!data.serverError;
}

export function isDeleted<T extends BaseDto>(data: WithOptimistic<T>): boolean {
    return data.isDeleted === true;
}
