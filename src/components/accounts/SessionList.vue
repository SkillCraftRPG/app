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
        <SessionCard :session="session" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import SessionCard from "./SessionCard.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Session } from "@/types/account";

const { t } = useI18n();

defineProps<{
  sessions: Session[];
}>();
</script>
