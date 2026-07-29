<template>
  <TarSelect
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  />
</template>

<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: boolean;
    placeholder?: string;
  }>(),
  {
    placeholder: "all",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: boolean | undefined): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      {
        text: t("no"),
        value: "false",
      },
      {
        text: t("yes"),
        value: "true",
      },
    ],
    "text",
  ),
);

function onModelValueUpdate(value: string | undefined): void {
  emit("update:model-value", parseBoolean(value));
}
</script>
