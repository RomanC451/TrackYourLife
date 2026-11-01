import { useCustomMutation } from "@/hooks/useCustomMutation";

import {
  createGuitarTabDevice,
  updateGuitarTabDeviceSettings,
  type GuitarTabDeviceSettings,
} from "../api/guitarTabsApi";
import { guitarTabsQueryKeys } from "../queries/guitarTabsQueries";

export function useCreateGuitarTabDeviceMutation() {
  return useCustomMutation({
    mutationFn: (
      request: GuitarTabDeviceSettings & { deviceId: string; name: string },
    ) => createGuitarTabDevice(request),
    meta: {
      onSuccessToast: { message: "Device saved", type: "success" },
      invalidateQueries: [guitarTabsQueryKeys.devices()],
      awaitInvalidationQuery: guitarTabsQueryKeys.devices(),
    },
  });
}

export function useUpdateGuitarTabDeviceSettingsMutation() {
  return useCustomMutation({
    mutationFn: ({
      deviceId,
      settings,
    }: {
      deviceId: string;
      settings: GuitarTabDeviceSettings;
    }) => updateGuitarTabDeviceSettings(deviceId, settings),
    meta: {
      onSuccessToast: { message: "Settings saved", type: "success" },
      invalidateQueries: [guitarTabsQueryKeys.devices()],
      awaitInvalidationQuery: guitarTabsQueryKeys.devices(),
    },
  });
}
