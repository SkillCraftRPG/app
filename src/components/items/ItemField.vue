<template>
  <SelectField
    :disabled="!options.length"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { Item } from "@/types/items";
import type { SearchResults } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";
import { listItems } from "@/api/items";

const { orderBy } = arrayUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "item",
    label: "items.label",
    placeholder: "items.placeholder",
  },
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Item | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const isLoading = ref<boolean>(false);
const items = ref<Item[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    items.value.map(({ id, name }) => ({ text: name, value: id })),
    "text",
  ),
);

function onModelValueUpdate(value: string | undefined): void {
  emit("update:model-value", value ?? "");

  const item: Item | undefined = items.value.find((item) => item.id === value);
  emit("selected", item);
}

async function refresh(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const results: SearchResults<Item> = await listItems();
      items.value = [...results.items];
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
