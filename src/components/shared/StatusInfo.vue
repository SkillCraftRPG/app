<template>
  <span>
    {{ t(format, { date: formattedDate }) }}
    <span>
      <TarAvatar
        :display-name="displayName"
        :email-address="actor.emailAddress ?? undefined"
        :icon="icon"
        :size="24"
        :url="actor.pictureUrl ?? undefined"
        :variant="variant"
      />
      {{ displayName }}
    </span>
  </span>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarAvatar from "@/components/tar/TarAvatar.vue";
import type { Actor } from "@/types/api";
import type { BadgeVariant } from "@/types/tar/badge";
import { formatRelativeTime } from "@/utils/date";

const { d, locale, t } = useI18n();
const { parseBoolean } = parsingUtils;

const props = defineProps<{
  actor: Actor;
  date: string;
  format: string;
  relative?: boolean | string;
}>();

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
