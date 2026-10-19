import { useState, type ReactNode } from "react";

import type { GuitarTabChord, GuitarTabContent } from "../api/guitarTabsApi";
import { displayChordName, transposeGuitarChord } from "../lib/transposeChord";
import GuitarTabAutoscroll from "./GuitarTabAutoscroll";
import GuitarTabTranspose from "./GuitarTabTranspose";

const stringGap = 12;
const fretGap = 16;
const diagramLeft = 18;
const diagramTop = 18;

function ChordDiagram({ chord }: { chord: GuitarTabChord }) {
  const lowToHigh = [...chord.frets].reverse();
  const start = chord.position > 0 ? chord.position : 1;
  const width = diagramLeft + stringGap * 5 + 10;
  const height = diagramTop + fretGap * 4 + 6;

  const barre =
    chord.barreFret != null &&
    chord.barreFromString != null &&
    chord.barreToString != null
      ? {
          x1: diagramLeft + (5 - Math.max(chord.barreFromString, chord.barreToString)) * stringGap,
          x2: diagramLeft + (5 - Math.min(chord.barreFromString, chord.barreToString)) * stringGap,
          y: diagramTop + (chord.barreFret - start + 0.5) * fretGap,
        }
      : null;

  return (
    <figure className="flex flex-col items-center gap-1">
      <figcaption className="text-sm font-medium">{chord.name}</figcaption>
      <svg
        width={width}
        height={height}
        viewBox={`0 0 ${width} ${height}`}
        aria-hidden
      >
        {chord.position > 0 && (
          <text
            x={2}
            y={diagramTop + fretGap * 0.7}
            fontSize={10}
            fill="currentColor"
          >
            {chord.position}
          </text>
        )}
        {Array.from({ length: 5 }, (_, fret) => (
          <line
            key={`fret-${fret}`}
            x1={diagramLeft}
            x2={diagramLeft + stringGap * 5}
            y1={diagramTop + fret * fretGap}
            y2={diagramTop + fret * fretGap}
            stroke="currentColor"
            strokeWidth={fret === 0 && chord.position === 0 ? 3 : 1}
          />
        ))}
        {lowToHigh.map((_, stringIndex) => (
          <line
            key={`string-${stringIndex}`}
            x1={diagramLeft + stringIndex * stringGap}
            x2={diagramLeft + stringIndex * stringGap}
            y1={diagramTop}
            y2={diagramTop + fretGap * 4}
            stroke="currentColor"
          />
        ))}
        {barre && (
          <line
            x1={barre.x1}
            x2={barre.x2}
            y1={barre.y}
            y2={barre.y}
            stroke="currentColor"
            strokeWidth={6}
            strokeLinecap="round"
          />
        )}
        {lowToHigh.map((fret, stringIndex) => {
          const x = diagramLeft + stringIndex * stringGap;
          if (fret < 0) {
            return (
              <text
                key={stringIndex}
                x={x}
                y={diagramTop - 4}
                textAnchor="middle"
                fontSize={10}
                fill="currentColor"
              >
                x
              </text>
            );
          }

          if (fret === 0) {
            return (
              <circle
                key={stringIndex}
                cx={x}
                cy={diagramTop - 6}
                r={3}
                fill="none"
                stroke="currentColor"
              />
            );
          }

          return (
            <circle
              key={stringIndex}
              cx={x}
              cy={diagramTop + (fret - start + 0.5) * fretGap}
              r={4}
              className="fill-amber-400"
            />
          );
        })}
      </svg>
    </figure>
  );
}

function renderLine(
  line: string,
  semitones: number,
  useFlats: boolean,
): ReactNode {
  const withoutTabTags = line.replace(/\[\/?tab\]/g, "");
  const section = withoutTabTags.trim().match(/^\[([^[\]]+)\]$/);
  if (section) {
    return <span className="font-sans text-base font-semibold">{section[1]}</span>;
  }

  const parts = withoutTabTags.split(/(\[ch\].*?\[\/ch\])/g);
  return parts.map((part, index) => {
    const chord = part.match(/^\[ch\](.*?)\[\/ch\]$/);
    if (!chord) {
      return <span key={index}>{part}</span>;
    }

    return (
      <span key={index} className="text-amber-400">
        {displayChordName(chord[1], semitones, useFlats)}
      </span>
    );
  });
}

function performanceDetails(tab: GuitarTabContent, tonality: string | null) {
  const tuning =
    tab.tuning == null
      ? null
      : tab.tuningName && tab.tuningName !== "Standard"
        ? `Tuning ${tab.tuningName} (${tab.tuning})`
        : `Tuning ${tab.tuning}`;

  return [
    tonality ? `Key ${tonality}` : null,
    tab.capo != null ? `Capo ${tab.capo}` : null,
    tuning,
  ]
    .filter((part) => part != null)
    .join(" · ");
}

function GuitarTabContentView({
  tab,
  actions,
  player,
}: {
  tab: GuitarTabContent;
  actions?: ReactNode;
  player?: ReactNode;
}) {
  const [semitones, setSemitones] = useState(0);
  const [useFlats, setUseFlats] = useState(false);
  const lines = tab.content.replace(/\r\n/g, "\n").split("\n");
  const tonality = tab.tonality
    ? displayChordName(tab.tonality, semitones, useFlats)
    : null;
  const chords = tab.chords.map((chord) =>
    transposeGuitarChord(chord, semitones, useFlats),
  );
  const details = performanceDetails(tab, tonality);

  return (
    <div className="grid items-start gap-6 lg:grid-cols-[minmax(0,1fr)_24rem]">
      <div className="order-2 flex flex-col gap-6 lg:order-1">
      <div className="flex items-start justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold">{tab.songName}</h2>
          <p className="text-muted-foreground">
            {tab.artistName}
            {" · "}
            {tab.type}
          </p>
          {details && <p className="text-muted-foreground">{details}</p>}
        </div>
        {actions}
      </div>
      {chords.length > 0 && (
        <div className="flex flex-wrap gap-4">
          {chords.map((chord, index) => (
            <ChordDiagram key={`${chord.name}-${index}`} chord={chord} />
          ))}
        </div>
      )}
      <div className="sticky top-0 z-10 flex flex-col gap-2 bg-background/95 py-2 backdrop-blur">
        <div className="flex flex-wrap items-center gap-3">
          <GuitarTabAutoscroll />
          <GuitarTabTranspose
            semitones={semitones}
            useFlats={useFlats}
            onSemitonesChange={setSemitones}
            onUseFlatsChange={setUseFlats}
          />
        </div>
        <GuitarTabStrummingPlayer patterns={tab.strummings ?? []} tonality={tonality} />
      </div>
      <div className="overflow-x-auto font-mono text-sm leading-6">
        {lines.map((line, index) => (
          <div key={index} className="whitespace-pre">
            {line.trim().length === 0 ? " " : renderLine(line, semitones, useFlats)}
          </div>
        ))}
      </div>
      </div>
      {player && (
        <div className="order-1 lg:sticky lg:top-4 lg:order-2 lg:self-start">{player}</div>
      )}
    </div>
  );
}

export default GuitarTabContentView;
