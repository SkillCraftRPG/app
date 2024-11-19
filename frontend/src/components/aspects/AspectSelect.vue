<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { AspectModel, SearchAspectsPayload } from "@/types/aspects";
import type { SearchResults } from "@/types/search";
import type { ValidationType } from "@/types/validation";
import { searchAspects } from "@/api/aspects";

const props = defineProps<{
  disabled?: boolean | string;
  exclude?: (string | AspectModel)[];
  modelValue?: string;
  validation?: ValidationType;
}>();

const aspects = ref<AspectModel[]>([]);
const hasLoaded = ref<boolean>(false);

const excludedIds = computed<Set<string>>(() => new Set<string>(props.exclude?.map((aspect) => (typeof aspect === "string" ? aspect : aspect.id))));
const options = computed<SelectOption[]>(() => aspects.value.filter(({ id }) => !excludedIds.value.has(id)).map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: AspectModel): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string) {
  const aspect: AspectModel | undefined = aspects.value.find((aspect) => aspect.id === id);
  emit("selected", aspect);
  emit("update:model-value", id);
}

onMounted(async () => {
  try {
    const payload: SearchAspectsPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<AspectModel> = await searchAspects(payload);
    aspects.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    hasLoaded.value = true;
  }
});
</script>

<template>
  <AppSelect
    :disabled="disabled || !hasLoaded"
    floating
    id="aspect"
    label="aspects.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="aspects.select.placeholder"
    :validation="validation"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </AppSelect>
</template>
