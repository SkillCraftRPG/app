<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { CustomizationModel, CustomizationType } from "@/types/customizations";
import type { SearchResults } from "@/types/search";
import type { ValidationType } from "@/types/validation";
import { searchCustomizations } from "@/api/customizations";

const props = defineProps<{
  modelValue?: string;
  type?: CustomizationType;
  validation?: ValidationType;
}>();

const customizations = ref<CustomizationModel[]>([]);

const options = computed<SelectOption[]>(() => customizations.value.map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: CustomizationModel): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string) {
  const customization: CustomizationModel | undefined = customizations.value.find((customization) => customization.id === id);
  emit("selected", customization);
  emit("update:model-value", id);
}

onMounted(async () => {
  try {
    const results: SearchResults<CustomizationModel> = await searchCustomizations({
      ids: [],
      search: { terms: [], operator: "And" },
      type: props.type,
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    });
    customizations.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <AppSelect
    floating
    id="customization"
    label="customizations.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="customizations.select.placeholder"
    :validation="validation"
    @update:model-value="onModelValueUpdate"
  />
</template>
