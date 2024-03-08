export function getClosestMultiple(number: number, multiple: number) {
  return Math.round(number / multiple) * multiple;
}

export function getPercentages(array: number[]) {
  const total = array.reduce((a, b) => a + b, 0);
  return array.map((value) => Math.round((value / total) * 100));
}
