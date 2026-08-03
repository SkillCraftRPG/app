<template>
  <TarSelect
    :disabled="!options.length"
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </TarSelect>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { Language } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";
import { listLanguages } from "@/api/languages";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    exclude?: string[];
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
  }>(),
  {
    exclude: () => [],
    id: "language",
    label: "languages.label",
    placeholder: "all",
  },
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Language | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const isLoading = ref<boolean>(false);
const languages = ref<Language[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    languages.value.filter((language) => !props.exclude.includes(language.id)).map(({ id, name }) => ({ text: name, value: id })),
    "text",
  ),
);

function onModelValueUpdate(value: string | undefined): void {
  emit("update:model-value", value ?? "");

  const language: Language | undefined = languages.value.find((language) => language.id === value);
  emit("selected", language);
}

async function refresh(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const results: SearchResults<Language> = await listLanguages();
      languages.value = [...results.items];
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
defineExpose({ refresh });

onMounted(refresh);
</script>
