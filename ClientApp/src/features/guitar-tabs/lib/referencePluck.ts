export const REFERENCE_PLUCK_SECONDS = 1.5;

export function synthesizePluck(
  frequency: number,
  sampleRate: number,
): Float32Array {
  const length = Math.max(1, Math.floor(sampleRate * REFERENCE_PLUCK_SECONDS));
  const samples = new Float32Array(length);
  const period = Math.max(2, Math.round(sampleRate / frequency));

  for (let i = 0; i < period && i < length; i++) {
    samples[i] = Math.random() * 2 - 1;
  }

  for (let i = period; i < length; i++) {
    const previous = samples[i - period];
    const neighbor = samples[i - period + 1] ?? previous;
    samples[i] = ((previous + neighbor) / 2) * 0.996;
  }

  for (let i = 0; i < length; i++) {
    samples[i] *= Math.exp((-3 * i) / length);
  }

  const fade = Math.min(length, Math.floor(sampleRate * 0.02));
  for (let i = 0; i < fade; i++) {
    const gain = i / fade;
    samples[i] *= gain;
    samples[length - 1 - i] *= gain;
  }

  return samples;
}

export function playReferencePluck(
  frequency: number,
  audioContext: AudioContext,
): { stop: () => void; finished: Promise<void> } {
  const samples = synthesizePluck(frequency, audioContext.sampleRate);
  const buffer = audioContext.createBuffer(
    1,
    samples.length,
    audioContext.sampleRate,
  );
  buffer.copyToChannel(samples, 0);

  const source = audioContext.createBufferSource();
  source.buffer = buffer;
  const gain = audioContext.createGain();
  gain.gain.value = 0.45;
  source.connect(gain);
  gain.connect(audioContext.destination);

  let resolveFinished = () => {};
  const finished = new Promise<void>((resolve) => {
    resolveFinished = resolve;
  });
  source.onended = () => resolveFinished();
  source.start();

  let stopped = false;
  return {
    stop: () => {
      if (stopped) {
        return;
      }
      stopped = true;
      source.stop();
    },
    finished,
  };
}
