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
import type { SearchResults } from "@/types/search";
import type { SearchTalentsPayload, Talent } from "@/types/talents";
import type { SelectOption } from "@/types/tar/select";
import { searchTalents } from "@/api/talents";

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
    id: "talent",
    label: "talents.label",
    placeholder: "all",
  },
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Talent | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const isLoading = ref<boolean>(false);
const talents = ref<Talent[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    talents.value.map(({ id, name }) => ({ text: name, value: id })),
    "text",
  ),
);

function onModelValueUpdate(value: string | undefined): void {
  emit("update:model-value", value ?? "");

  const talent: Talent | undefined = talents.value.find((talent) => talent.id === value);
  emit("selected", talent);
}

async function refresh(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: SearchTalentsPayload = {
        ids: [],
        search: { terms: [], operator: "And" },
        sort: [{ field: "Name", isDescending: false }],
        skip: 0,
        limit: 0,
      };
      const results: SearchResults<Talent> = await searchTalents(payload);
      talents.value = [...results.items];
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
