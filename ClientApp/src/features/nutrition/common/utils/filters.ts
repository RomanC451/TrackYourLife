import { ServingSizeDto } from "@/services/openapi";

export function getServingSizeIndex(
  servingSizes: { [key: string]: ServingSizeDto },
  servingSize: ServingSizeDto,
) {
  return Object.values(servingSizes).findIndex(
    (ss) => ss.id === servingSize.id,
  );
}
