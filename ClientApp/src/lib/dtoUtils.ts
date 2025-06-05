/**
 * Sets isManuallySet to true for a DTO and all its nested DTOs
 * @param dto The DTO to set isManuallySet on
 * @returns The modified DTO
 */
export const setManuallySet = <T>(dto: T): T => {
  if (dto === null || dto === undefined) {
    return dto;
  }

  if (Array.isArray(dto)) {
    return dto.map(setManuallySet) as T;
  }

  if (typeof dto === 'object' && dto !== null) {
    const result = { ...dto } as Record<string, unknown>;
    
    // Check if this is a DTO (ends with Dto)
    if (Object.keys(result).includes('id')) {
      result.isManuallySet = false;
    }

    // Recursively process all properties
    for (const key in result) {
      result[key] = setManuallySet(result[key]);
    }
    
    return result as T;
  }

  return dto;
}; 