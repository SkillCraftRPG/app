<template>
  <div class="d-flex gap-2">
    <TarBadge v-if="mode === 'None'" pill variant="secondary">
      <font-awesome-icon icon="fas fa-xmark" />&nbsp;{{ t("account.multiFactorAuthentication.disabled") }}
    </TarBadge>
    <template v-else>
      <TarBadge pill variant="success"> <font-awesome-icon icon="fas fa-check" />&nbsp;{{ t("account.multiFactorAuthentication.enabled") }} </TarBadge>
      <TarBadge v-if="mode === 'Email'" pill variant="secondary">
        <font-awesome-icon icon="fas fa-envelope" />&nbsp;{{ t("account.multiFactorAuthentication.email.label") }}
      </TarBadge>
      <TarBadge v-else-if="mode === 'Phone'" pill variant="secondary">
        <font-awesome-icon icon="fas fa-comment-sms" />&nbsp;{{ t("account.multiFactorAuthentication.phone") }}
      </TarBadge>
    </template>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";
import type { MultiFactorAuthenticationMode } from "@/types/account";

const { t } = useI18n();

defineProps<{
  mode: MultiFactorAuthenticationMode;
}>();
</script>
