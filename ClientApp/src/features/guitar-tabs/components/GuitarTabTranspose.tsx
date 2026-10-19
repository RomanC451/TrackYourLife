import { Minus, Plus } from "lucide-react";

import { Button } from "@/components/ui/button";

import { changeTranspose } from "../lib/transposeChord";

function GuitarTabTranspose({
  semitones,
  useFlats,
  onSemitonesChange,
  onUseFlatsChange,
}: {
  semitones: number;
  useFlats: boolean;
  onSemitonesChange: (semitones: number) => void;
  onUseFlatsChange: (useFlats: boolean) => void;
}) {
  const adjust = (direction: -1 | 1) => {
    onSemitonesChange(changeTranspose(semitones, direction));
  };

  return (
    <div className="flex items-center gap-1" data-transpose="">
      <div
        role="spinbutton"
        aria-label="Transpose"
        aria-valuenow={semitones}
        aria-valuemin={-11}
        aria-valuemax={11}
        aria-valuetext={`Tr. ${semitones}`}
        tabIndex={0}
        className="flex items-center gap-1"
        onKeyDown={(event) => {
          if (event.key === "ArrowUp" || event.key === "ArrowRight") {
            event.preventDefault();
            adjust(1);
          }
          if (event.key === "ArrowDown" || event.key === "ArrowLeft") {
            event.preventDefault();
            adjust(-1);
          }
        }}
      >
        <Button
          type="button"
          size="icon"
          variant="outline"
          className="size-8"
          aria-label="Transpose Down"
          onClick={() => adjust(-1)}
        >
          <Minus />
        </Button>
        <span className="w-14 text-center text-sm tabular-nums">Tr. {semitones}</span>
        <Button
          type="button"
          size="icon"
          variant="outline"
          className="size-8"
          aria-label="Transpose Up"
          onClick={() => adjust(1)}
        >
          <Plus />
        </Button>
      </div>
      <Button
        type="button"
        size="sm"
        variant={useFlats ? "default" : "outline"}
        aria-pressed={useFlats}
        aria-label="Use Flats"
        onClick={() => onUseFlatsChange(!useFlats)}
      >
        ♭
      </Button>
    </div>
  );
}

export default GuitarTabTranspose;
