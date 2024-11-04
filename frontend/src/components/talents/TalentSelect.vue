<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, ref, watchEffect } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { SearchTalentsPayload, TalentModel } from "@/types/talents";
import type { SearchResults } from "@/types/search";
import type { ValidationType } from "@/types/validation";
import { searchTalents } from "@/api/talents";

const props = withDefaults(
  defineProps<{
    label?: string;
    maxTier?: number;
    modelValue?: string;
    validation?: ValidationType;
  }>(),
  {
    label: "talents.select.label",
  },
);

const hasLoaded = ref<boolean>(false);
const talents = ref<TalentModel[]>([]);

const options = computed<SelectOption[]>(() => talents.value.map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: TalentModel): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string) {
  const talent: TalentModel | undefined = talents.value.find((talent) => talent.id === id);
  emit("selected", talent);
  emit("update:model-value", id);
}

watchEffect(async () => {
  const maxTier: number | undefined = props.maxTier;
  try {
    const payload: SearchTalentsPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      tier: typeof maxTier === "number" ? { values: [maxTier], operator: "lte" } : undefined,
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<TalentModel> = await searchTalents(payload);
    talents.value = results.items;
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
    id="talent"
    :label="label"
    :model-value="modelValue"
    :options="options"
    placeholder="talents.select.placeholder"
    :validation="validation"
    @update:model-value="onModelValueUpdate"
  />
</template>
