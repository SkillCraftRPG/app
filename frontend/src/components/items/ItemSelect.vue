<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { ItemModel, SearchItemsPayload } from "@/types/items";
import type { SearchResults } from "@/types/search";
import { searchItems } from "@/api/items";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
  }>(),
  {
    id: "item",
    label: "items.select.label",
  },
);

const items = ref<ItemModel[]>([]);
const hasLoaded = ref<boolean>(false);

const options = computed<SelectOption[]>(() => items.value.map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: ItemModel): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string) {
  const item: ItemModel | undefined = items.value.find((item) => item.id === id);
  emit("selected", item);
  emit("update:model-value", id);
}

onMounted(async () => {
  try {
    const payload: SearchItemsPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<ItemModel> = await searchItems(payload);
    items.value = results.items;
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
    placeholder="items.select.placeholder"
    @update:model-value="onModelValueUpdate"
  />
</template>
