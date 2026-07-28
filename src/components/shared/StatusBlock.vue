<template>
  <div>
    <div>
      {{ t(format, { date: formattedDate }) }}
    </div>
    <div>
      {{ t("by") }}
      <TarAvatar
        :display-name="displayName"
        :email-address="actor.emailAddress ?? undefined"
        :icon="icon"
        :size="24"
        :url="actor.pictureUrl ?? undefined"
        :variant="variant"
      />
      {{ displayName }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarAvatar from "@/components/tar/TarAvatar.vue";
import { formatRelativeTime } from "@/utils/date";
import type { Actor } from "@/types/api";
import type { BadgeVariant } from "@/types/tar/badge";

const { parseBoolean } = parsingUtils;
const { d, locale, t } = useI18n();

const props = withDefaults(
  defineProps<{
    actor: Actor;
    format?: string;
    date: Date | string;
    relative?: boolean | string;
  }>(),
  {
    format: "status.updatedOn",
  },
);

const displayName = computed<string>(() => {
  const { displayName, type } = props.actor;
  return type === "System" ? t("system") : displayName;
});
const formattedDate = computed<string>(() =>
  parseBoolean(props.relative) ? formatRelativeTime(props.date, `${locale.value}-CA`, t("status.now")) : d(props.date, "medium"),
);
const icon = computed<string | undefined>(() => {
  switch (props.actor.type) {
    case "ApiKey":
      return "fas fa-key";
    case "User":
      return "fas fa-user";
  }
  return "fas fa-robot";
});
const variant = computed<BadgeVariant | undefined>(() => (props.actor.type === "ApiKey" ? "info" : undefined));
</script>
