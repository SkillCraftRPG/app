<template>
  <div>
    <div class="form-label">{{ t(label) }}</div>
    <div class="d-flex flex-wrap gap-3">
      <div v-for="option in options" :key="option.code" class="form-check">
        <input
          :checked="modelValue === option.code"
          class="form-check-input"
          :id="`${id}-${option.code}`"
          :name="id"
          :required="isRequired"
          type="radio"
          :value="option.code"
          @change="$emit('update:model-value', option.code)"
        />
        <label class="form-check-label" :for="`${id}-${option.code}`">{{ option.nativeName }}</label>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import locales from "@/assets/data/locales.json";
import type { Locale } from "@/types/i18n";

const preferredOrder: string[] = ["fr", "en"];
const { availableLocales, t } = useI18n();
const { parseBoolean } = parsingUtils;

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "locale",
    label: "account.locale.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const isRequired = computed<boolean>(() => parseBoolean(props.required) ?? false);
const options = computed<Locale[]>(() => {
  const supported = new Set<string>(availableLocales);
  const byCode = new Map<string, Locale>(locales.map((locale) => [locale.code, locale]));
  const ordered: Locale[] = [];

  for (const code of preferredOrder) {
    const locale = byCode.get(code);
    if (locale && supported.has(code)) {
      ordered.push(locale);
    }
  }

  for (const code of availableLocales) {
    if (preferredOrder.includes(code)) {
      continue;
    }
    const locale = byCode.get(code);
    if (locale) {
      ordered.push(locale);
    }
  }

  return ordered;
});
</script>
