<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { PersonalityModel, SearchPersonalitiesPayload } from "@/types/personalities";
import type { SearchResults } from "@/types/search";
import { searchPersonalities } from "@/api/personalities";

defineProps<{
  modelValue?: string;
  required?: boolean | string;
}>();

const hasLoaded = ref<boolean>(false);
const personalities = ref<PersonalityModel[]>([]);

const options = computed<SelectOption[]>(() => personalities.value.map(({ id, name }) => ({ text: name, value: id })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: PersonalityModel): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string) {
  const personality: PersonalityModel | undefined = personalities.value.find((personality) => personality.id === id);
  emit("selected", personality);
  emit("update:model-value", id);
}

onMounted(async () => {
  try {
    const payload: SearchPersonalitiesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<PersonalityModel> = await searchPersonalities(payload);
    personalities.value = results.items;
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
    id="personality"
    label="personalities.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="personalities.select.placeholder"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>
