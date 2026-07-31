<template>
  <form @submit.prevent="handleSubmit(submit)">
    <NameField class="mb-3" required v-model="name" />
    <SummaryField class="mb-3" v-model="summary" />
    <ContentField class="mb-3" v-model="content" />
    <div class="d-flex justify-content-end mb-3">
      <TarButton
        :disabled="!hasChanges || isLoading"
        icon="fas fa-floppy-disk"
        :loading="isLoading"
        size="large"
        :status="t('loading')"
        :text="t('actions.save')"
        type="submit"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import ContentField from "@/components/shared/ContentField.vue";
import NameField from "@/components/shared/NameField.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Lineage, UpdateLineagePayload } from "@/types/lineages";
import { updateLineage } from "@/api/lineages";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Lineage): void;
}>();

const content = ref<string>("");
const isLoading = ref<boolean>(false);
const name = ref<string>("");
const summary = ref<string>("");

const hasChanges = computed<boolean>(() =>
  Boolean(props.lineage.name !== name.value || (props.lineage.summary ?? "") !== summary.value || (props.lineage.content ?? "") !== content.value),
);

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateLineagePayload = {
        name: name.value,
        summary: { value: summary.value || null },
        content: { value: content.value || null },
      };
      const lineage: Lineage = await updateLineage(props.lineage.id, payload);
      reinitialize();
      emit("updated", lineage);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.lineage,
  (lineage) => {
    name.value = lineage?.name ?? "";
    summary.value = lineage?.summary ?? "";
    content.value = lineage?.content ?? "";
  },
  { deep: true, immediate: true },
);
</script>
