<template>
  <div>
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-start gap-3 mb-3">
      <div>
        <h3 class="h5 mb-1">{{ t("account.sessions.list") }}</h3>
        <p class="text-body-secondary mb-0">
          {{ t("account.sessions.help") }}
        </p>
      </div>
      <TarButton icon="fas fa-arrow-right-from-bracket" :text="t('account.signOut.all.submit')" variant="danger" />
    </div>
    <div class="row">
      <div v-for="session in sessions" :key="session.id" class="col-md-6 col-lg-4 mb-3">
        <SessionCard :session="session" @error="$emit('error', $event)" @signed-out="onSignOut(session)" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import SessionCard from "./SessionCard.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Session } from "@/types/account";
import type { SearchResults } from "@/types/search";
import { listActiveSessions } from "@/api/sessions";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "signed-out", value: Session): void;
}>();

const sessions = ref<Session[]>([]);

function onSignOut(session: Session): void {
  const index: number = sessions.value.findIndex(({ id }) => id === session.id);
  if (index >= 0) {
    sessions.value.splice(index, 1);
  }
  toasts.success("account.sessions.signedOut");
}

onMounted(async () => {
  try {
    const results: SearchResults<Session> = await listActiveSessions();
    sessions.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>
