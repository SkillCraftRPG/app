<template>
  <div class="d-flex">
    <div class="d-flex">
      <div class="d-flex align-content-center flex-wrap mx-1">
        <TarAvatar :display-name="displayName" :email-address="actor.emailAddress ?? ''" :icon="icon" :url="actor.pictureUrl ?? ''" :variant="variant" />
      </div>
    </div>
    <div>
      {{ d(date, "medium") }}
      <br />
      {{ t("by") }}
      {{ displayName }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarAvatar from "@/components/tar/TarAvatar.vue";
import type { Actor } from "@/types/api";
import type { BadgeVariant } from "@/types/tar/badge";

const { d, t } = useI18n();

const props = defineProps<{
  actor: Actor;
  date: string;
}>();

const displayName = computed<string>(() => {
  const { displayName, type } = props.actor;
  return type === "System" ? t("system") : displayName;
});
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
