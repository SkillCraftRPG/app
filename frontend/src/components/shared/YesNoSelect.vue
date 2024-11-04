<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { ValidationType } from "@/types/validation";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

defineProps<{
  id?: string;
  label?: string;
  modelValue?: boolean | string;
  validation?: ValidationType;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      { text: t("no"), value: "false" },
      { text: t("yes"), value: "true" },
    ],
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: boolean): void;
}>();
</script>

<template>
  <AppSelect
    floating
    :id="id"
    :label="label"
    :model-value="modelValue?.toString()"
    :options="options"
    placeholder="select.placeholder"
    :validation="validation"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseBoolean($event))"
  />
</template>
