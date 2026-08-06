<template>
  <TarSelect
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { SelectOption } from "@/types/tar/select";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: number;
    placeholder?: string;
  }>(),
  {
    id: "tier",
    label: "talents.tier.label",
    placeholder: "all",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();

const maxTier = computed<number | undefined>(() => parseNumber(props.max));
const options = computed<SelectOption[]>(() =>
  [0, 1, 2, 3]
    .filter((tier) => typeof maxTier.value !== "number" || tier <= maxTier.value)
    .map((value) => ({ text: value.toString(), value: value.toString() })),
);

function onModelValueUpdate(value: string | undefined): void {
  emit("update:model-value", parseNumber(value || undefined));
}
</script>
