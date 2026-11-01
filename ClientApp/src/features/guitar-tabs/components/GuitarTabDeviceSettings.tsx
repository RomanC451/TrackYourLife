import { useEffect, useRef, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { useLocalStorage } from "usehooks-ts";
import { v4 as uuidv4 } from "uuid";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import type { GuitarTabDeviceSettings } from "../api/guitarTabsApi";
import {
  useCreateGuitarTabDeviceMutation,
  useUpdateGuitarTabDeviceSettingsMutation,
} from "../mutations/useGuitarTabDeviceMutations";
import { guitarTabsQueryOptions } from "../queries/guitarTabsQueries";

const defaultSettings: GuitarTabDeviceSettings = {
  speed: 0.2,
  semitones: 0,
  useFlats: false,
};

function sameSettings(
  left: GuitarTabDeviceSettings,
  right: GuitarTabDeviceSettings,
) {
  return (
    Math.round(left.speed * 10) === Math.round(right.speed * 10) &&
    left.semitones === right.semitones &&
    left.useFlats === right.useFlats
  );
}

function GuitarTabDeviceSettingsBar({
  speed,
  semitones,
  useFlats,
  onApply,
}: {
  speed: number;
  semitones: number;
  useFlats: boolean;
  onApply: (settings: GuitarTabDeviceSettings) => void;
}) {
  const [deviceId] = useLocalStorage("deviceId", uuidv4(), {
    serializer: (value) => value,
    deserializer: (value) => value,
  });
  const devicesQuery = useQuery(guitarTabsQueryOptions.devices());
  const devices = devicesQuery.data ?? [];
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [saved, setSaved] = useState<GuitarTabDeviceSettings | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const initialized = useRef(false);
  const onApplyRef = useRef(onApply);
  onApplyRef.current = onApply;
  const createDevice = useCreateGuitarTabDeviceMutation();
  const updateSettings = useUpdateGuitarTabDeviceSettingsMutation();

  const current = { speed, semitones, useFlats };
  const selected = devices.find((device) => device.deviceId === selectedId);
  const currentDeviceSaved = devices.some((device) => device.deviceId === deviceId);
  const dirty = !sameSettings(saved ?? defaultSettings, current);

  useEffect(() => {
    if (!devicesQuery.isSuccess || initialized.current) {
      return;
    }

    initialized.current = true;
    const match = devicesQuery.data?.find((device) => device.deviceId === deviceId);
    if (!match) {
      return;
    }

    const settings = {
      speed: match.speed,
      semitones: match.semitones,
      useFlats: match.useFlats,
    };
    setSelectedId(match.deviceId);
    setSaved(settings);
    onApplyRef.current(settings);
  }, [deviceId, devicesQuery.data, devicesQuery.isSuccess]);

  const selectDevice = (id: string) => {
    const device = devices.find((item) => item.deviceId === id);
    if (!device) {
      return;
    }

    const settings = {
      speed: device.speed,
      semitones: device.semitones,
      useFlats: device.useFlats,
    };
    setSelectedId(device.deviceId);
    setSaved(settings);
    onApplyRef.current(settings);
  };

  const save = async () => {
    if (!selectedId) {
      setDialogOpen(true);
      return;
    }

    await updateSettings.mutateAsync({ deviceId: selectedId, settings: current });
    setSaved(current);
  };

  const create = async (name: string) => {
    await createDevice.mutateAsync({
      deviceId,
      name,
      ...current,
    });
    setSelectedId(deviceId);
    setSaved(current);
    setDialogOpen(false);
  };

  return (
    <div className="flex flex-wrap items-center gap-2">
      <Label htmlFor="guitar-tab-device">Device</Label>
      <Select
        value={selectedId ?? "unset"}
        onValueChange={selectDevice}
        disabled={devices.length === 0}
      >
        <SelectTrigger id="guitar-tab-device" className="w-56" aria-label="Device">
          <SelectValue placeholder="Select a device">{selected?.name}</SelectValue>
        </SelectTrigger>
        <SelectContent>
          {devices.map((device) => (
            <SelectItem key={device.deviceId} value={device.deviceId}>
              {device.name}
              {device.deviceId === deviceId ? " · this device" : ""}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
      {devicesQuery.isFetched && !currentDeviceSaved && (
        <Button type="button" variant="outline" onClick={() => setDialogOpen(true)}>
          New device
        </Button>
      )}
      {dirty && (
        <>
          <Button
            type="button"
            onClick={() => void save()}
            disabled={updateSettings.isPending || createDevice.isPending}
          >
            Save settings
          </Button>
          <Button
            type="button"
            variant="outline"
            onClick={() => onApply(saved ?? defaultSettings)}
          >
            Reset settings
          </Button>
        </>
      )}
      {selected && selected.deviceId !== deviceId && (
        <p className="text-sm text-muted-foreground">
          These settings are saved for {selected.name}.
        </p>
      )}
      {dialogOpen && (
        <NewDeviceDialog
          pending={createDevice.isPending}
          onOpenChange={setDialogOpen}
          onCreate={create}
        />
      )}
    </div>
  );
}

function NewDeviceDialog({
  pending,
  onOpenChange,
  onCreate,
}: {
  pending: boolean;
  onOpenChange: (open: boolean) => void;
  onCreate: (name: string) => Promise<void>;
}) {
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);

  const submit = async () => {
    const trimmed = name.trim();
    if (!trimmed) {
      setError("Name is required.");
      return;
    }

    try {
      await onCreate(trimmed);
    } catch {
      setError("Couldn't save this device.");
    }
  };

  return (
    <Dialog open onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>New device</DialogTitle>
          <DialogDescription>
            This name is saved with this browser, using the same device as sign-in.
            Opening the app here loads these settings.
          </DialogDescription>
        </DialogHeader>
        <div className="grid gap-2">
          <Label htmlFor="guitar-tab-device-name">Name</Label>
          <Input
            id="guitar-tab-device-name"
            value={name}
            onChange={(event) => setName(event.target.value)}
            autoFocus
          />
          {error && <p className="text-sm text-destructive">{error}</p>}
        </div>
        <DialogFooter>
          <Button type="button" onClick={() => void submit()} disabled={pending}>
            Create
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

export default GuitarTabDeviceSettingsBar;
