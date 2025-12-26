import { useEffect, useState } from "react";
import { X } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dropzone,
  DropzoneContent,
  DropzoneEmptyState,
} from "@/components/ui/shadcn-io/dropzone";
import { useCustomMutation } from "@/hooks/useCustomMutation";

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

const FileDropZone = ({
  uploadFunction,
  onSuccess,
  onRemove,
  defaultImageUrl,
  setIsPending,
}: {
  uploadFunction: (file: File) => Promise<string>;
  onSuccess: (data: string) => void;
  onRemove?: () => void;
  defaultImageUrl?: string;
  setIsPending?: (isPending: boolean) => void;
}) => {
  const mutation = useUploadFileMutation(uploadFunction);

  useEffect(() => {
    if (setIsPending) {
      setIsPending(mutation.isPending);
    }
  }, [mutation.isPending, setIsPending]);

  useEffect(() => {}, []);

  const [files, setFiles] = useState<File[] | undefined>();
  const [filePreview, setFilePreview] = useState<string | undefined>();
  const handleDrop = (files: File[]) => {
    console.log(files);
    setFiles(files);
    mutation.mutate(files[0], {
      onSuccess: (data) => {
        onSuccess(data);
      },
    });
    if (files.length > 0) {
      const reader = new FileReader();
      reader.onload = (e) => {
        if (typeof e.target?.result === "string") {
          setFilePreview(e.target?.result);
        }
      };
      reader.readAsDataURL(files[0]);
    }
  };

  function renderImageContent() {
    if (filePreview) {
      return (
        <div className="h-[102px] w-full">
          <img
            alt="Preview"
            className="absolute left-0 top-0 h-full w-full object-cover"
            src={filePreview}
          />
          <Button
            variant="destructive"
            type="button"
            size="icon"
            className="absolute right-2 top-2 z-10 rounded-full opacity-70"
            onClick={(e) => {
              e.stopPropagation();
              setFiles(undefined);
              setFilePreview(undefined);
              onRemove?.();
            }}
          >
            <X className="" />
          </Button>
        </div>
      );
    }
    if (defaultImageUrl) {
      return (
        <div className="h-[102px] w-full">
          <img
            alt="Preview"
            className="absolute left-0 top-0 h-full w-full object-cover"
            src={defaultImageUrl}
          />
          <Button
            variant="destructive"
            type="button"
            size="icon"
            className="absolute right-2 top-2 z-10 rounded-full opacity-70"
            onClick={(e) => {
              e.stopPropagation();
              setFiles(undefined);
              setFilePreview(undefined);
              onRemove?.();
            }}
          >
            <X className="" />
          </Button>
        </div>
      );
    }
    return null;
  }

  return (
    <Dropzone
      accept={{ "image/*": [".png", ".jpg", ".jpeg"] }}
      onDrop={handleDrop}
      onError={console.error}
      src={files}
    >
      <DropzoneEmptyState />
      <DropzoneContent className="relative">
        {renderImageContent()}
      </DropzoneContent>
    </Dropzone>
  );
};
export default FileDropZone;
