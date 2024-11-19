<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { CasteModel, SearchCastesPayload } from "@/types/castes";
import type { SearchResults } from "@/types/search";
import { searchCastes } from "@/api/castes";

defineProps<{
  modelValue?: string;
  required?: boolean | string;
}>();

const castes = ref<CasteModel[]>([]);
const hasLoaded = ref<boolean>(false);

const options = computed<SelectOption[]>(() => castes.value.map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: CasteModel): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string) {
  const caste: CasteModel | undefined = castes.value.find((caste) => caste.id === id);
  emit("selected", caste);
  emit("update:model-value", id);
}

onMounted(async () => {
  try {
    const payload: SearchCastesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<CasteModel> = await searchCastes(payload);
    castes.value = results.items;
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
    id="caste"
    label="castes.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="castes.select.placeholder"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>
