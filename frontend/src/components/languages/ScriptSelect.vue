<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { SearchResults } from "@/types/search";
import { listScripts } from "@/api/languages";

defineProps<{
  modelValue?: string;
}>();

const scripts = ref<string[]>([]);

const options = computed<SelectOption[]>(() => scripts.value.map((script) => ({ text: script, value: script })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value?: string): void;
}>();

onMounted(async () => {
  try {
    const results: SearchResults<string> = await listScripts();
    scripts.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <AppSelect
    floating
    id="script"
    label="languages.script.label"
    :model-value="modelValue"
    :options="options"
    placeholder="languages.script.all"
    validation="server"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
