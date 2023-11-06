export default function getClosestMultiple(number: number, multiple: number) {
  return Math.round(number / multiple) * multiple;
}
