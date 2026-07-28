<template>
  <TarCard :class="classes">
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
    <div class="d-flex justify-content-end align-items-center mt-auto pt-3">
      <TarBadge v-if="session.isCurrent" pill variant="primary">{{ t("account.sessions.current") }}</TarBadge>
      <TarButton
        v-else
        :disabled="isLoading"
        icon="fas fa-arrow-right-from-bracket"
        :loading="isLoading"
        outline
        :status="t('loading')"
        type="button"
        :text="t('account.signOut.title')"
        variant="secondary"
        @click="executeSignOut"
      />
    </div>
  </TarCard>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { Session } from "@/types/account";
import { formatRelativeTime } from "@/utils/date";
import { signOutById } from "@/api/sessions";

const { d, locale, t } = useI18n();

const props = defineProps<{
  session: Session;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "signed-out"): void;
}>();

const isLoading = ref<boolean>(false);

const classes = computed<string[]>(() => {
  const classes: string[] = ["session-card", "h-100"];
  if (props.session.isCurrent) {
    classes.push("border-primary");
  }
  return classes;
});
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
const updatedOn = computed<string>(() => formatRelativeTime(props.session.updatedOn, `${locale.value}-CA`, t("status.now")));

async function executeSignOut(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      await signOutById(props.session.id);
      emit("signed-out");
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<style scoped>
.session-card :deep(.card-body) {
  display: flex;
  flex-direction: column;
  height: 100%;
}
</style>
