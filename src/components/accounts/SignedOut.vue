<template>
  <TarAlert
    :close="t('actions.close')"
    :dismissible="isDismissible"
    :model-value="modelValue"
    variant="success"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <strong>{{ t("account.signOut.lead") }}</strong> {{ t("account.signOut.help") }}
  </TarAlert>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarAlert from "@/components/tar/TarAlert.vue";

const { t } = useI18n();

const props = defineProps<{
  modelValue?: boolean;
}>();

defineEmits<{
  (e: "update:model-value", value: false): void;
}>();

const isDismissible = computed<boolean>(() => typeof props.modelValue === "boolean");
</script>
