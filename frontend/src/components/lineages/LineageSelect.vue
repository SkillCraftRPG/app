<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { LineageModel, SearchLineagesPayload } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import { searchLineages } from "@/api/lineages";
import type { ValidationType } from "@/types/validation";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
    validation?: ValidationType;
  }>(),
  {
    id: "lineage",
    label: "lineages.select.label",
    placeholder: "lineages.select.placeholder",
  },
);

const hasLoaded = ref<boolean>(false);
const lineages = ref<LineageModel[]>([]);

const options = computed<SelectOption[]>(() => lineages.value.map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: LineageModel): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string) {
  const lineage: LineageModel | undefined = lineages.value.find((lineage) => lineage.id === id);
  emit("selected", lineage);
  emit("update:model-value", id);
}

onMounted(async () => {
  try {
    const payload: SearchLineagesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<LineageModel> = await searchLineages(payload);
    lineages.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    hasLoaded.value = true;
  }
});
</script>

<template>
  <AppSelect
    :disabled="!hasLoaded"
    floating
    :id="id"
    :label="label"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    :validation="validation"
    @update:model-value="onModelValueUpdate"
  />
</template>
