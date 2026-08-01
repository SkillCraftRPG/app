<template>
  <SelectField
    :disabled="!options.length"
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

import SelectField from "@/components/forms/SelectField.vue";
import type { SearchResults } from "@/types/search";
import type { SearchTalentsPayload, Talent } from "@/types/talents";
import type { SelectOption } from "@/types/tar/select";
import { searchTalents } from "@/api/talents";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    exclude?: string[];
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
    tiers?: number[];
  }>(),
  {
    exclude: () => [],
    id: "talent",
    label: "talents.label",
    placeholder: "talents.placeholder",
    tiers: () => [],
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
    talents.value.filter((talent) => !props.exclude.includes(talent.id)).map(({ id, name }) => ({ text: name, value: id })),
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
        tiers: props.tiers,
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
