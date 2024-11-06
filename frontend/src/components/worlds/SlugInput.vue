<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";

const { slugify } = stringUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue?: string;
  name?: string;
}>();

const isCustom = ref<boolean>(false);

const emit = defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();

function setIsCustom(value: boolean): void {
  isCustom.value = value;
  if (!value) {
    emit("update:model-value", slugify(props.name));
  }
}

watch(
  () => props.name,
  (name) => {
    if (!isCustom.value) {
      emit("update:model-value", slugify(name));
    }
  },
  { immediate: true },
);
</script>

<template>
  <AppInput
    :disabled="!isCustom"
    floating
    id="slug"
    label="worlds.slug.label"
    max="255"
    :model-value="modelValue"
    placeholder="worlds.slug.label"
    required
    :rules="{ slug: true }"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <TarCheckbox id="is-custom" :label="t('worlds.slug.custom')" :model-value="isCustom" @update:model-value="setIsCustom" />
    </template>
  </AppInput>
</template>
