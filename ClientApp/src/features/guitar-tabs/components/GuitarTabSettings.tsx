import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";

import {
  setGuitarTabPlaybackSettings,
  useGuitarTabPlaybackSettings,
} from "../lib/playbackSettings";

function SettingRow({
  id,
  title,
  description,
  checked,
  onCheckedChange,
}: {
  id: string;
  title: string;
  description: string;
  checked: boolean;
  onCheckedChange: (checked: boolean) => void;
}) {
  return (
    <div className="flex items-start gap-3">
      <Checkbox
        id={id}
        checked={checked}
        onCheckedChange={(value) => onCheckedChange(value === true)}
        className="mt-1"
      />
      <div className="grid gap-1">
        <Label htmlFor={id}>{title}</Label>
        <p className="text-sm text-muted-foreground">{description}</p>
      </div>
    </div>
  );
}

function GuitarTabSettings() {
  const settings = useGuitarTabPlaybackSettings();

  return (
    <div className="grid max-w-xl gap-6">
      <SettingRow
        id="guitar-tabs-autoscroll"
        title="Autoscroll"
        description="Space starts and stops scrolling the tab."
        checked={settings.autoscroll}
        onCheckedChange={(autoscroll) => setGuitarTabPlaybackSettings({ autoscroll })}
      />
      <SettingRow
        id="guitar-tabs-sync-video"
        title="Sync video"
        description="Space starts and stops the video together with autoscroll."
        checked={settings.syncVideo}
        onCheckedChange={(syncVideo) => setGuitarTabPlaybackSettings({ syncVideo })}
      />
    </div>
  );
}

export default GuitarTabSettings;
