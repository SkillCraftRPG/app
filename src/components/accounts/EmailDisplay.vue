<template>
  <TarInput floating :label="label" :model-value="email.address" :placeholder="label" plaintext readonly>
    <template #append v-if="email.isVerified">
      <span class="input-group-text border-0 bg-transparent pe-0">
        <TarBadge pill variant="success"><font-awesome-icon icon="fas fa-check" />&nbsp;{{ t("account.email.verified") }}</TarBadge>
      </span>
    </template>
  </TarInput>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";
import TarInput from "@/components/tar/TarInput.vue";
import type { Email } from "@/types/account";

const { t } = useI18n();

defineProps<{
  email: Email;
}>();

const label = computed<string>(() => t("account.email.address"));
</script>
