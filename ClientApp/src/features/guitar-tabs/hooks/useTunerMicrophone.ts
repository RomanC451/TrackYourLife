import { useCallback, useEffect, useRef, useState } from "react";

import { detectPitch } from "../lib/pitchDetection";

export type MicrophoneStatus = "requesting" | "listening" | "denied" | "insecure";

export function localhostTunerHref(location: {
  port: string;
  pathname: string;
  search: string;
}) {
  const port = location.port ? `:${location.port}` : "";
  return `http://127.0.0.1${port}${location.pathname}${location.search}`;
}

function microphoneIsBlockedByAddress() {
  return typeof window !== "undefined" && window.isSecureContext === false;
}

const PITCH_INTERVAL_MS = 50;
const DOWNSAMPLE = 4;

function downsample(samples: Float32Array, factor: number) {
  const length = Math.floor(samples.length / factor);
  const down = new Float32Array(length);
  for (let i = 0; i < length; i++) {
    let sum = 0;
    const offset = i * factor;
    for (let j = 0; j < factor; j++) {
      sum += samples[offset + j];
    }
    down[i] = sum / factor;
  }
  return down;
}

export function useTunerMicrophone() {
  const [status, setStatus] = useState<MicrophoneStatus>(() =>
    microphoneIsBlockedByAddress() ? "insecure" : "requesting",
  );
  const [pitchHz, setPitchHz] = useState<number | null>(null);
  const [pitchFrame, setPitchFrame] = useState(0);
  const stopRef = useRef<() => void>(() => {});
  const requestId = useRef(0);

  const request = useCallback(() => {
    if (microphoneIsBlockedByAddress()) {
      setStatus("insecure");
      return;
    }

    stopRef.current();

    const id = ++requestId.current;
    let stream: MediaStream | null = null;
    let audioContext: AudioContext | null = null;
    let frame = 0;
    let stopped = false;
    let lastDetectAt = 0;

    const stop = () => {
      if (stopped) {
        return;
      }
      stopped = true;
      cancelAnimationFrame(frame);
      stream?.getTracks().forEach((track) => track.stop());
      void audioContext?.close();
    };
    stopRef.current = stop;
    setStatus("requesting");

    void (async () => {
      try {
        if (!navigator.mediaDevices?.getUserMedia) {
          throw new Error("Microphone unavailable");
        }
        stream = await navigator.mediaDevices.getUserMedia({
          audio: {
            echoCancellation: false,
            noiseSuppression: false,
            autoGainControl: false,
          },
        });
        if (stopped || id !== requestId.current) {
          stream.getTracks().forEach((track) => track.stop());
          return;
        }

        audioContext = new AudioContext();
        const source = audioContext.createMediaStreamSource(stream);
        const analyser = audioContext.createAnalyser();
        analyser.fftSize = 4096;
        source.connect(analyser);
        const samples = new Float32Array(analyser.fftSize);
        const sampleRate = audioContext.sampleRate;

        const tick = (time: number) => {
          if (stopped || id !== requestId.current) {
            return;
          }
          frame = requestAnimationFrame(tick);
          if (time - lastDetectAt < PITCH_INTERVAL_MS) {
            return;
          }
          lastDetectAt = time;
          analyser.getFloatTimeDomainData(samples);
          const nextPitch = detectPitch(
            downsample(samples, DOWNSAMPLE),
            sampleRate / DOWNSAMPLE,
          );
          setPitchFrame((frame) => frame + 1);
          setPitchHz((current) => {
            if (current == null && nextPitch == null) {
              return current;
            }
            if (
              current != null &&
              nextPitch != null &&
              Math.abs(current - nextPitch) < 0.05
            ) {
              return current;
            }
            return nextPitch;
          });
        };

        setStatus("listening");
        frame = requestAnimationFrame(tick);
      } catch {
        if (!stopped && id === requestId.current) {
          setStatus("denied");
          setPitchHz(null);
        }
      }
    })();
  }, []);

  useEffect(() => {
    if (microphoneIsBlockedByAddress()) {
      setStatus("insecure");
      return;
    }

    request();
    return () => stopRef.current();
  }, [request]);

  return { status, pitchHz, pitchFrame, request };
}
