import { useLayoutEffect, useRef, useState, type ReactNode } from "react";

import type { GuitarTabChord, GuitarTabContent } from "../api/guitarTabsApi";
import {
  chordSpans,
  layoutTab,
  spanText,
  trimTabLine,
  wrapTabLine,
  type TabSpan,
} from "../lib/tabLines";
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

function ChordSpans({ spans }: { spans: TabSpan[] }) {
  return spans.map((span, index) =>
    span.chord ? (
      <span key={index} className="text-amber-400">
        {span.text}
      </span>
    ) : (
      <span key={index}>{span.text}</span>
    ),
  );
}

function TabRows({
  spans,
  lyric,
  columns,
  chordsOnly = false,
}: {
  spans: TabSpan[];
  lyric: string;
  columns: number;
  chordsOnly?: boolean;
}) {
  const rows = wrapTabLine(spans, lyric, columns)
    .map(trimTabLine)
    .filter((row) => spanText(row.chord).length > 0 || row.lyric.length > 0);

  return rows.map((row, index) => (
    <div key={index}>
      {spanText(row.chord).length > 0 && (
        <div className="whitespace-pre">
          <ChordSpans spans={row.chord} />
        </div>
      )}
      {!chordsOnly && row.lyric.length > 0 && (
        <div className="whitespace-pre">{row.lyric}</div>
      )}
    </div>
  ));
}

function useSheetColumns() {
  const ref = useRef<HTMLDivElement>(null);
  const [columns, setColumns] = useState(0);

  useLayoutEffect(() => {
    const element = ref.current;
    if (!element) {
      return;
    }

    let active = true;
    const update = () => {
      const probe = element.querySelector("[data-tab-probe]");
      const charWidth = probe?.getBoundingClientRect().width ?? 0;
      if (!active || charWidth <= 0) {
        return;
      }

      setColumns(Math.max(1, Math.floor(element.clientWidth / charWidth)));
    };

    update();
    const observer =
      typeof ResizeObserver === "undefined"
        ? null
        : new ResizeObserver(update);
    observer?.observe(element);
    void document.fonts?.ready.then(() => {
      if (active) {
        update();
      }
    });

    return () => {
      active = false;
      observer?.disconnect();
    };
  }, []);

  return [ref, columns] as const;
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
  const [sheetRef, columns] = useSheetColumns();
  const blocks = layoutTab(tab.content);
  const tonality = tab.tonality
    ? displayChordName(tab.tonality, semitones, useFlats)
    : null;
  const chords = tab.chords.map((chord) =>
    transposeGuitarChord(chord, semitones, useFlats),
  );
  const details = performanceDetails(tab, tonality);

  return (
    <div className="grid items-start gap-6 lg:grid-cols-[minmax(0,1fr)_24rem]">
      <div className="order-2 flex min-w-0 flex-col gap-6 lg:order-1">
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
      </div>
      <div
        ref={sheetRef}
        className="relative w-full min-w-0 overflow-x-clip font-mono text-sm leading-6"
      >
        <span
          data-tab-probe=""
          className="pointer-events-none invisible absolute font-mono text-sm"
          aria-hidden
        >
          0
        </span>
        {blocks.map((block, index) => {
          if (block.kind === "blank") {
            return <div key={index} className="h-6" />;
          }

          if (block.kind === "section") {
            return (
              <div key={index} className="font-sans text-base font-semibold">
                {block.title}
              </div>
            );
          }

          if (block.kind === "staff") {
            return (
              <div key={index} className="overflow-x-auto whitespace-pre">
                {block.text}
              </div>
            );
          }

          if (block.kind === "text") {
            return (
              <div key={index} className="whitespace-pre-wrap">
                {block.text}
              </div>
            );
          }

          const line = block.kind === "pair" ? block.chordLine : block.line;
          const spans = chordSpans(line, semitones, useFlats);
          return (
            <TabRows
              key={index}
              spans={spans}
              lyric={block.kind === "pair" ? block.lyricLine : spanText(spans)}
              columns={columns}
              chordsOnly={block.kind === "chords"}
            />
          );
        })}
      </div>
      </div>
      {player && (
        <div className="order-1 lg:sticky lg:top-4 lg:order-2 lg:self-start">{player}</div>
      )}
    </div>
  );
}

export default GuitarTabContentView;
