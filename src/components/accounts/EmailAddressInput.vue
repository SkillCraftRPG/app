<template>
  <FormInput
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :required="required"
    type="email"
    @update:model-value="$emit('update:model-value', $event ?? '')"
  />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "email",
    label: "account.email.address",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>
