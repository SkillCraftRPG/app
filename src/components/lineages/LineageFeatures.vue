<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="mb-3">
      <TarButton icon="fas fa-plus" size="large" :text="t('actions.add')" @click="add" />
    </div>
    <template v-if="features.length">
      <EditLineageFeature
        v-for="(feature, index) in features"
        :key="index"
        class="mb-3"
        :id="`feature-${index}`"
        :model-value="feature"
        @remove="remove(index)"
        @update:model-value="update(index, $event)"
      >
      </EditLineageFeature>
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
    </template>
    <p v-else>{{ t("lineages.features.empty") }}</p>
  </form>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import EditLineageFeature from "./EditLineageFeature.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Feature } from "@/types/features";
import type { Lineage, UpdateLineagePayload } from "@/types/lineages";
import { useForm } from "@/forms";
import { updateLineage } from "@/api/lineages";

const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Lineage): void;
}>();

const features = ref<Feature[]>([]);
const isLoading = ref<boolean>(false);

const hasChanges = computed<boolean>(() => JSON.stringify(props.lineage.features) !== JSON.stringify(features.value));

function add(): void {
  features.value.push({ name: "" });
}
function remove(index: number): void {
  features.value.splice(index, 1);
}
function update(index: number, value: Feature): void {
  features.value.splice(index, 1, value);
}

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateLineagePayload = {
        features: features.value,
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
  (lineage) => (features.value = lineage.features.map((feature) => ({ ...feature }))),
  { deep: true, immediate: true },
);
</script>
