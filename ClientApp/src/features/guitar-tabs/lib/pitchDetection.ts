const MIN_FREQUENCY_HZ = 50;
const MAX_FREQUENCY_HZ = 500;
const MIN_RMS = 0.01;
const MIN_CORRELATION = 0.9;

function correlationAt(samples: Float32Array, lag: number): number {
  const limit = samples.length - lag;
  let correlation = 0;
  let energyA = 0;
  let energyB = 0;
  for (let i = 0; i < limit; i++) {
    const left = samples[i];
    const right = samples[i + lag];
    correlation += left * right;
    energyA += left * left;
    energyB += right * right;
  }
  const denominator = Math.sqrt(energyA * energyB);
  return denominator === 0 ? 0 : correlation / denominator;
}

export function detectPitch(
  samples: Float32Array,
  sampleRate: number,
): number | null {
  const length = samples.length;
  if (length < 32 || sampleRate <= 0) {
    return null;
  }

  let energy = 0;
  for (let i = 0; i < length; i++) {
    energy += samples[i] * samples[i];
  }
  if (Math.sqrt(energy / length) < MIN_RMS) {
    return null;
  }

  const minLag = Math.max(2, Math.floor(sampleRate / MAX_FREQUENCY_HZ));
  const maxLag = Math.min(length - 2, Math.floor(sampleRate / MIN_FREQUENCY_HZ));
  if (minLag >= maxLag) {
    return null;
  }

  const correlations = new Float64Array(maxLag + 2);
  let bestLag = minLag;
  let bestCorrelation = -1;
  for (let lag = minLag; lag <= maxLag; lag++) {
    const correlation = correlationAt(samples, lag);
    correlations[lag] = correlation;
    if (correlation > bestCorrelation) {
      bestCorrelation = correlation;
      bestLag = lag;
    }
  }

  if (bestCorrelation < MIN_CORRELATION) {
    return null;
  }

  let lag = bestLag;
  let improved = true;
  while (improved) {
    improved = false;
    const half = Math.round(lag / 2);
    if (half >= minLag && correlations[half] >= correlations[lag] - 0.05) {
      lag = half;
      improved = true;
    }
  }

  const previous = lag > minLag ? correlationAt(samples, lag - 1) : correlations[lag];
  const current = correlations[lag] || correlationAt(samples, lag);
  const next = correlationAt(samples, lag + 1);
  const denominator = previous - 2 * current + next;
  const shift = denominator === 0 ? 0 : (previous - next) / (2 * denominator);
  const refinedLag = lag + Math.max(-1, Math.min(1, shift));
  if (refinedLag <= 0) {
    return null;
  }

  return sampleRate / refinedLag;
}
