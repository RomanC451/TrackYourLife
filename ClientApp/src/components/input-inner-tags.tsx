"use client";

import React, { useEffect, useId, useRef, useState } from "react";
import { Tag, TagInput } from "emblor-maintained";
import { Check } from "lucide-react";

import { cn } from "@/lib/utils";

import { Card } from "./ui/card";
import { ScrollArea } from "./ui/scroll-area";

type InputInnerTagsProps = {
  initialTags: Tag[];
  autocompleteOptions: Tag[];
  placeholder: string;
  setTags: (tags: Tag[]) => void;
} & Omit<React.ComponentProps<"input">, "draggable">;

export default function InputInnerTags({
  initialTags = [],
  autocompleteOptions = [],
  placeholder = "Add a tag",
  setTags,
  ...inputProps
}: InputInnerTagsProps) {
  const id = useId();
  const [exampleTags, setExampleTags] = useState<Tag[]>(initialTags);
  const [activeTagIndex, setActiveTagIndex] = useState<number | null>(null);

  const [autocompleteOpen, setAutocompleteOpen] = useState(false);

  const [input, setInput] = useState("");

  const options = autocompleteOptions.filter((option) =>
    option.text.toLowerCase().startsWith(input.toLowerCase()),
  );

  const autocompleteRef = useRef<HTMLDivElement>(null);

  const addTagFromInput = (value: string) => {
    const trimmed = value.trim();
    if (!trimmed) {
      return false;
    }

    const normalized = trimmed.toLowerCase();
    const matchedOption = autocompleteOptions.find(
      (option) => option.text.toLowerCase() === normalized,
    );

    let didAdd = false;
    setExampleTags((prev) => {
      const alreadySelected = prev.some(
        (tag) => tag.text.toLowerCase() === normalized,
      );

      if (alreadySelected) {
        return prev;
      }

      if (matchedOption) {
        didAdd = true;
        return [...prev, matchedOption];
      }

      const id =
        typeof crypto !== "undefined" && "randomUUID" in crypto
          ? crypto.randomUUID()
          : `${trimmed}-${Date.now()}`;

      didAdd = true;
      return [
        ...prev,
        {
          id,
          text: trimmed,
        },
      ];
    });

    return didAdd;
  };

  const focusInput = () => {
    requestAnimationFrame(() => {
      const element = document.getElementById(id) as HTMLInputElement | null;
      element?.focus();
    });
  };

  const commitInputValue = (value: string) => {
    const didAdd = addTagFromInput(value);
    setInput("");
    if (didAdd) {
      focusInput();
    }
  };

  const toggleOption = (option: Tag) => {
    setExampleTags((prev) => {
      if (prev.includes(option)) {
        return prev.filter((tag) => tag.id !== option.id);
      }

      setInput("");

      return [...prev, option];
    });
  };

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (
        (event.target as HTMLElement)?.id !== ":ru:" &&
        autocompleteRef.current &&
        !autocompleteRef.current.contains(event.target as Node)
      ) {
        setAutocompleteOpen(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [setAutocompleteOpen]);

  useEffect(() => {
    setTags(exampleTags);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [exampleTags]);

  return (
    <div className="*:not-first:mt-2 pointer-events-auto relative">
      <TagInput
        id={id}
        tags={exampleTags}
        setTags={(newTags) => {
          setExampleTags(newTags);
        }}
        placeholder={placeholder}
        onFocus={() => setAutocompleteOpen(true)}
        styleClasses={{
          inlineTagsContainer:
            "border-input rounded-md bg-background shadow-xs transition-[color,box-shadow] focus-within:border-ring outline-none focus-within:ring-[3px] focus-within:ring-ring/50 p-1 gap-1",
          input: "w-full min-w-[80px] shadow-none px-2 h-7 ",
          tag: {
            body: "h-7 relative bg-background border border-input hover:bg-background rounded-md font-medium text-xs ps-2 pe-7",
            closeButton:
              "absolute -inset-y-px -end-px p-0 rounded-e-md flex size-7 transition-[color,box-shadow] outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] text-muted-foreground/80 hover:text-foreground",
          },
        }}
        activeTagIndex={activeTagIndex}
        setActiveTagIndex={setActiveTagIndex}
        inputProps={{
          ...inputProps,
          value: input,
          enterKeyHint: "done",

          // onKeyDownCapture: (e) => {
          //   if (e.key === "Enter") {
          //     e.preventDefault();
          //     e.stopPropagation();
          //     commitInputValue(e.currentTarget.value);
          //   }
          // },
          onKeyDown: (e) => {
            if (e.key === "Enter") {
              e.preventDefault();
              e.stopPropagation();
              commitInputValue(e.currentTarget.value);
            }
          },
        }}
        onInputChange={(e) => {
          setInput(e);
        }}
      />
      {autocompleteOptions.length > 0 && autocompleteOpen && (
        <Card
          className={cn(
            "absolute left-0 mt-2 w-full p-2 backdrop-blur-xl",
            options.length > 0 ? "h-[200px]" : "h-[45px]",
          )}
          ref={autocompleteRef}
        >
          <ScrollArea className="h-full" type="always">
            <div className="flex flex-col">
              {options.length > 0 ? (
                options.map((option) => (
                  <React.Fragment key={option.id}>
                    <button
                      type="button"
                      onClick={() => toggleOption(option)}
                      className="flex w-full items-center gap-4 rounded-md p-2 text-left text-sm hover:bg-secondary"
                    >
                      {option.text}
                      {exampleTags.includes(option) && (
                        <Check className="size-4" />
                      )}
                    </button>
                  </React.Fragment>
                ))
              ) : (
                <div className="flex h-full flex-col items-center justify-center gap-2">
                  <p className="text-muted-foreground">Press enter to add.</p>
                </div>
              )}
            </div>
          </ScrollArea>
        </Card>
      )}
    </div>
  );
}
