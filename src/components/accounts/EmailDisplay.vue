<template>
  <div>
    <div class="fw-bold">{{ t("account.email.address") }}</div>
    <div class="d-flex gap-2">
      <div>{{ email.address }}</div>
      <div v-if="email.isVerified">
        <TarBadge pill variant="success"><font-awesome-icon icon="fas fa-check" />&nbsp;{{ t("account.email.verified") }}</TarBadge>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";
import type { Email } from "@/types/account";

const { t } = useI18n();

defineProps<{
  email: Email;
}>();
</script>
