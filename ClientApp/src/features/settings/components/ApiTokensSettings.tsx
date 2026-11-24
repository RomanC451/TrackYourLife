import { useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { Check, Copy, KeyRound, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import type { CreatedPersonalAccessTokenDto } from "@/services/openapi";

import {
  useCreatePersonalAccessTokenMutation,
  useRevokePersonalAccessTokenMutation,
} from "../mutations/usePersonalAccessTokenMutations";
import { personalAccessTokensQueryOptions } from "../queries/personalAccessTokensQuery";

function formatDate(value?: string | null) {
  if (!value) {
    return "—";
  }

  return new Date(value).toLocaleString();
}

export default function ApiTokensSettings() {
  const { data: tokens } = useSuspenseQuery(
    personalAccessTokensQueryOptions.list(),
  );
  const createMutation = useCreatePersonalAccessTokenMutation();
  const revokeMutation = useRevokePersonalAccessTokenMutation();

  const [name, setName] = useState("");
  const [createdToken, setCreatedToken] =
    useState<CreatedPersonalAccessTokenDto | null>(null);
  const [copied, setCopied] = useState(false);

  async function handleCreate(event: React.FormEvent) {
    event.preventDefault();
    const trimmed = name.trim();
    if (!trimmed) {
      toast.error("Enter a name for this token");
      return;
    }

    const result = await createMutation.mutateAsync({ name: trimmed });
    setCreatedToken(result);
    setName("");
    setCopied(false);
    toast.success("Token created — copy it now, it won't be shown again");
  }

  async function handleCopy() {
    if (!createdToken?.token) {
      return;
    }

    await navigator.clipboard.writeText(createdToken.token);
    setCopied(true);
    toast.success("Token copied");
  }

  return (
    <div className="mx-auto grid max-w-2xl gap-8">
      <section className="space-y-3">
        <div className="space-y-1">
          <h2 className="text-base font-semibold tracking-tight">
            Personal access tokens
          </h2>
          <p className="text-sm text-muted-foreground">
            Create a long-lived token for Home Assistant or other integrations.
            The full token is shown only once after creation.
          </p>
        </div>

        <form
          onSubmit={handleCreate}
          className="flex flex-col gap-3 sm:flex-row sm:items-end"
        >
          <div className="grid flex-1 gap-1.5">
            <Label htmlFor="pat-name">Token name</Label>
            <Input
              id="pat-name"
              value={name}
              onChange={(event) => setName(event.target.value)}
              placeholder="Home Assistant"
              maxLength={100}
              autoComplete="off"
            />
          </div>
          <Button type="submit" disabled={createMutation.isPending}>
            <KeyRound />
            Create token
          </Button>
        </form>

        {createdToken?.token ? (
          <div className="space-y-2 rounded-md border border-border bg-muted/40 p-3">
            <p className="text-sm font-medium">New token for {createdToken.name}</p>
            <div className="flex flex-col gap-2 sm:flex-row">
              <Input
                readOnly
                value={createdToken.token}
                className="font-mono text-xs"
              />
              <Button type="button" variant="outline" onClick={handleCopy}>
                {copied ? <Check /> : <Copy />}
                {copied ? "Copied" : "Copy"}
              </Button>
            </div>
            <p className="text-xs text-muted-foreground">
              Store this token securely. You will not be able to view it again.
            </p>
          </div>
        ) : null}
      </section>

      <section className="space-y-3">
        <h3 className="text-sm font-semibold tracking-tight">Active tokens</h3>
        {tokens.length === 0 ? (
          <p className="text-sm text-muted-foreground">No tokens yet.</p>
        ) : (
          <ul className="divide-y divide-border rounded-md border border-border">
            {tokens.map((token) => (
              <li
                key={token.id}
                className="flex flex-col gap-3 p-3 sm:flex-row sm:items-center sm:justify-between"
              >
                <div className="space-y-1">
                  <p className="text-sm font-medium">{token.name}</p>
                  <p className="font-mono text-xs text-muted-foreground">
                    {token.tokenPrefix}…
                  </p>
                  <p className="text-xs text-muted-foreground">
                    Created {formatDate(token.createdOnUtc)} · Last used{" "}
                    {formatDate(token.lastUsedOnUtc)} · Expires{" "}
                    {formatDate(token.expiresAt)}
                  </p>
                </div>
                <Button
                  type="button"
                  variant="destructive"
                  size="sm"
                  disabled={revokeMutation.isPending}
                  onClick={() => {
                    if (
                      window.confirm(
                        `Revoke “${token.name}”? Integrations using this token will stop working.`,
                      )
                    ) {
                      revokeMutation.mutate(token.id);
                      if (createdToken?.id === token.id) {
                        setCreatedToken(null);
                      }
                    }
                  }}
                >
                  <Trash2 />
                  Revoke
                </Button>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}
