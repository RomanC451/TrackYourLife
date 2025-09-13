import { useEffect } from "react";
import { CircleUserRoundIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { useFileUpload } from "@/hooks/use-file-upload";
import { useCustomMutation } from "@/hooks/useCustomMutation";
import { cn } from "@/lib/utils";

import Spinner from "./spinner";

function useUploadFileMutation(
  uploadFunction: (file: File) => Promise<string>,
) {
  const mutation = useCustomMutation({
    mutationFn: (file: File) => {
      return uploadFunction(file);
    },
    meta: {
      onSuccessToast: {
        message: "File uploaded successfully",
        type: "success",
      },
      onErrorToast: {
        message: "Failed to upload file",
        type: "error",
      },
      invalidateQueries: null,
    },
  });
  return mutation;
}

export default function UploadFileInput2({
  uploadFunction,
  onSuccess,
  onRemove,
  defaultImageUrl,
  defaultName,
  setIsPending,
}: {
  uploadFunction: (file: File) => Promise<string>;
  onSuccess: (data: string) => void;
  onRemove?: () => void;
  defaultImageUrl?: string;
  defaultName?: string;
  setIsPending?: (isPending: boolean) => void;
}) {
  const mutation = useUploadFileMutation(uploadFunction);

  useEffect(() => {
    if (setIsPending) {
      setIsPending(mutation.isPending);
    }
  }, [mutation.isPending, setIsPending]);

  const [{ files }, { removeFile, openFileDialog, getInputProps }] =
    useFileUpload({
      accept: "image/*",
      multiple: false,
      maxFiles: 1,
      maxSize: 1024 * 1024 * 5, // 5MB
      onFilesAdded: (files) => {
        mutation.mutate(files[0].file as File, {
          onSuccess: (data) => {
            onSuccess(data);
          },
        });
      },
    });

  const previewUrl = files[0]?.preview || null;
  const fileName = files[0]?.file.name || null;

  const renderImageContent = () => {
    if (previewUrl) {
      return (
        <div className="absolute">
          <img
            className={cn("size-full object-cover", {
              "opacity-50": mutation.isPending,
            })}
            src={previewUrl}
            alt="Preview of uploaded file"
            width={32}
            height={32}
          />
          {mutation.isPending && (
            <Spinner className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2" />
          )}
        </div>
      );
    }

    if (defaultImageUrl) {
      return (
        <img
          className="size-full object-cover"
          src={defaultImageUrl}
          alt="Default user avatar"
        />
      );
    }

    return (
      <div aria-hidden="true">
        <CircleUserRoundIcon className="opacity-60" size={16} />
      </div>
    );
  };

  return (
    <div className="flex flex-col items-start gap-2">
      <div className="inline-flex items-center gap-2 align-top">
        <div
          className="relative flex size-9 shrink-0 items-center justify-center overflow-hidden rounded-md border border-input"
          aria-label={
            previewUrl ? "Preview of uploaded image" : "Default user avatar"
          }
        >
          {renderImageContent()}
        </div>
        <div className="relative inline-block">
          <Button
            onClick={openFileDialog}
            aria-haspopup="dialog"
            variant="secondary"
            type="button"
          >
            {fileName ? "Change image" : "Upload image"}
          </Button>
          <input
            {...getInputProps()}
            className="sr-only"
            aria-label="Upload image file"
            tabIndex={-1}
          />
        </div>
      </div>
      {(fileName || defaultName) && (
        <div className="inline-flex gap-2 text-xs">
          <p className="truncate text-muted-foreground" aria-live="polite">
            {fileName || defaultName}
          </p>{" "}
          <button
            onClick={() => {
              removeFile(files[0]?.id);
              onRemove?.();
            }}
            type="button"
            className="font-medium text-destructive hover:underline"
            aria-label={`Remove ${fileName || defaultName}`}
          >
            Remove
          </button>
        </div>
      )}
    </div>
  );
}
