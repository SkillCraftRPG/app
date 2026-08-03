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
  />
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { Script } from "@/types/scripts";
import type { SearchResults } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";
import { listScripts } from "@/api/scripts";

const { orderBy } = arrayUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
  }>(),
  {
    id: "script",
    label: "scripts.label",
    placeholder: "all",
  },
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Script | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const isLoading = ref<boolean>(false);
const scripts = ref<Script[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    scripts.value.map(({ id, name }) => ({ text: name, value: id })),
    "text",
  ),
);

function onModelValueUpdate(value: string | undefined): void {
  emit("update:model-value", value ?? "");

  const script: Script | undefined = scripts.value.find((script) => script.id === value);
  emit("selected", script);
}

async function refresh(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const results: SearchResults<Script> = await listScripts();
      scripts.value = [...results.items];
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
