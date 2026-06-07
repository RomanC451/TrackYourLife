import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { formatBytes, useFileUpload } from "../use-file-upload";

describe("formatBytes", () => {
  it("formats byte values", () => {
    expect(formatBytes(0)).toBe("0 Bytes");
    expect(formatBytes(1024)).toBe("1KB");
    expect(formatBytes(1048576)).toBe("1MB");
  });
});

beforeEach(() => {
  URL.createObjectURL = vi.fn(() => "blob:preview");
  URL.revokeObjectURL = vi.fn();
});

describe("useFileUpload", () => {
  it("initializes with provided files", () => {
    const { result } = renderHook(() =>
      useFileUpload({
        initialFiles: [
          {
            id: "file-1",
            name: "photo.png",
            size: 100,
            type: "image/png",
            url: "https://example.com/photo.png",
          },
        ],
      }),
    );

    expect(result.current[0].files).toHaveLength(1);
    expect(result.current[0].files[0].id).toBe("file-1");
  });

  it("rejects files that exceed maxSize", () => {
    const onFilesAdded = vi.fn();
    const file = new File(["hello"], "large.txt", { type: "text/plain" });
    Object.defineProperty(file, "size", { value: 2048 });

    const { result } = renderHook(() =>
      useFileUpload({
        maxSize: 1024,
        onFilesAdded,
      }),
    );

    act(() => {
      result.current[1].addFiles([file]);
    });

    expect(result.current[0].files).toHaveLength(0);
    expect(result.current[0].errors[0]).toContain("maximum size");
    expect(onFilesAdded).not.toHaveBeenCalled();
  });

  it("replaces the existing file in single-file mode", () => {
    const first = new File(["one"], "one.txt", { type: "text/plain" });
    const second = new File(["two"], "two.txt", { type: "text/plain" });

    const { result } = renderHook(() => useFileUpload({ multiple: false }));

    act(() => {
      result.current[1].addFiles([first]);
    });
    act(() => {
      result.current[1].addFiles([second]);
    });

    expect(result.current[0].files).toHaveLength(1);
    expect(result.current[0].files[0].file).toBe(second);
  });

  it("removes a file by id", () => {
    const file = new File(["hello"], "note.txt", { type: "text/plain" });
    const { result } = renderHook(() => useFileUpload({ multiple: true }));

    act(() => {
      result.current[1].addFiles([file]);
    });

    const fileId = result.current[0].files[0].id;

    act(() => {
      result.current[1].removeFile(fileId);
    });

    expect(result.current[0].files).toHaveLength(0);
  });
});
