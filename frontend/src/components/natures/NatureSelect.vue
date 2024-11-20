<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { NatureModel, SearchNaturesPayload } from "@/types/natures";
import type { SearchResults } from "@/types/search";
import { searchNatures } from "@/api/natures";

defineProps<{
  modelValue?: string;
  required?: boolean | string;
}>();

const hasLoaded = ref<boolean>(false);
const natures = ref<NatureModel[]>([]);

const options = computed<SelectOption[]>(() => natures.value.map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: NatureModel): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string) {
  const nature: NatureModel | undefined = natures.value.find((nature) => nature.id === id);
  emit("selected", nature);
  emit("update:model-value", id);
}

onMounted(async () => {
  try {
    const payload: SearchNaturesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<NatureModel> = await searchNatures(payload);
    natures.value = results.items;
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
    id="nature"
    label="natures.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="natures.select.placeholder"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>
