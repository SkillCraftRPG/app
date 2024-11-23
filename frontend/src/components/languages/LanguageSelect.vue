<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { LanguageModel, SearchLanguagesPayload } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import type { ValidationType } from "@/types/validation";
import { searchLanguages } from "@/api/languages";

const props = defineProps<{
  disabled?: boolean | string;
  exclude?: (string | LanguageModel)[];
  modelValue?: string;
  required?: boolean | string;
  validation?: ValidationType;
}>();

const hasLoaded = ref<boolean>(false);
const languages = ref<LanguageModel[]>([]);

const excludedIds = computed<Set<string>>(() => new Set<string>(props.exclude?.map((language) => (typeof language === "string" ? language : language.id))));
const options = computed<SelectOption[]>(() =>
  languages.value.filter(({ id }) => !excludedIds.value.has(id)).map(({ id, name }) => ({ text: name, value: id })),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: LanguageModel): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string) {
  const language: LanguageModel | undefined = languages.value.find((language) => language.id === id);
  emit("selected", language);
  emit("update:model-value", id);
}

onMounted(async () => {
  try {
    const payload: SearchLanguagesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<LanguageModel> = await searchLanguages(payload);
    languages.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    hasLoaded.value = true;
  }
});
</script>

<template>
  <AppSelect
    :disabled="disabled || !hasLoaded"
    floating
    id="language"
    label="languages.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="languages.select.placeholder"
    :required="required"
    :validation="validation"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </AppSelect>
</template>
