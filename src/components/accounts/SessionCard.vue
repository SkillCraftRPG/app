<template>
  <TarCard class="h-100" :class="{ 'border-primary': session.isCurrent }">
    <div v-if="deviceIcon || deviceText" class="d-flex align-items-center gap-2">
      <font-awesome-icon v-if="deviceIcon" :icon="deviceIcon" aria-hidden="true" />
      <span v-if="deviceText">{{ deviceText }}</span>
    </div>
    <div class="d-flex align-items-center gap-2">
      <span><font-awesome-icon icon="fas fa-clock-rotate-left" /> {{ t("account.sessions.updatedOn") }}</span>
      <span>{{ updatedOn }}</span>
    </div>
    <div class="d-flex align-items-center gap-2">
      <span><font-awesome-icon icon="fas fa-arrow-right-to-bracket" /> {{ t("account.sessions.createdOn") }}</span>
      <span>{{ d(session.createdOn, "medium") }}</span>
    </div>
    <div class="d-flex align-items-center gap-2">
      <span><font-awesome-icon icon="fas fa-network-wired" /> {{ t("account.sessions.ipAddress") }}</span>
      <span>{{ session.ipAddress }}</span>
    </div>
    <div class="d-flex align-items-center mt-auto pt-3">
      <TarBadge v-if="session.isCurrent" pill variant="primary">{{ t("account.sessions.current") }}</TarBadge>
      <TarButton
        class="ms-auto"
        icon="fas fa-arrow-right-from-bracket"
        outline
        type="button"
        :text="t('account.signOut.title')"
        :variant="session.isCurrent ? 'danger' : 'secondary'"
        @click="$emit('sign-out')"
      />
    </div>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { Session } from "@/types/account";
import { formatRelativeTime } from "@/utils/date";

const { d, locale, t } = useI18n();

const props = defineProps<{
  session: Session;
}>();

const deviceIcon = computed<string>(() => {
  switch (props.session.deviceType) {
    case "Desktop":
    case "Mobile":
    case "Tablet":
      return `fas fa-${props.session.deviceType.toLowerCase()}`;
    default:
      return "";
  }
});
const deviceText = computed<string>(() => {
  const parts: string[] = [];
  if (props.session.browser) {
    parts.push(props.session.browser);
  }
  if (props.session.browser && props.session.operatingSystem) {
    parts.push(t("account.sessions.on"));
  }
  if (props.session.operatingSystem) {
    parts.push(props.session.operatingSystem);
  }
  return parts.join(" ");
});
const updatedOn = computed<string>(() => formatRelativeTime(props.session.updatedOn, `${locale.value}-CA`, t("account.sessions.now")));
</script>
