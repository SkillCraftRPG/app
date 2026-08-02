<template>
  <TarCard>
    <div class="card-text">
      <NameField class="mb-3" :id="`${id}-name`" :model-value="modelValue.name" required @update:model-value="updateName" />
      <ContentField class="mb-3" :id="`${id}-content`" :model-value="modelValue.content ?? ''" rows="7" @update:model-value="updateContent" />
    </div>
    <div class="d-flex justify-content-end">
      <TarButton icon="fas fa-xmark" outline :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
    </div>
  </TarCard>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import ContentField from "@/components/shared/ContentField.vue";
import NameField from "@/components/shared/NameField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { Feature } from "@/types/features";

const { t } = useI18n();

const props = defineProps<{
  id: string;
  modelValue: Feature;
}>();

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "update:model-value", value: Feature): void;
}>();

function updateContent(content: string): void {
  emit("update:model-value", { ...props.modelValue, content });
}
function updateName(name: string): void {
  emit("update:model-value", { ...props.modelValue, name });
}
</script>
