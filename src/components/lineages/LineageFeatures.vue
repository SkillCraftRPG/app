<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="mb-3">
      <TarButton icon="fas fa-plus" size="large" :text="t('actions.add')" @click="add" />
    </div>
    <template v-if="features.length">
      <TarCard v-for="(feature, index) in features" :key="index" class="mb-3">
        <div class="card-text">
          <NameField class="mb-3" :id="`feature-${index}-name`" :model-value="feature.name" required @update:model-value="setName(index, $event)" />
          <ContentField
            class="mb-3"
            :id="`feature-${index}-content`"
            :model-value="feature.content ?? ''"
            rows="7"
            @update:model-value="setContent(index, $event)"
          />
        </div>
        <div class="d-flex justify-content-end">
          <TarButton icon="fas fa-xmark" outline :text="t('actions.remove')" variant="danger" @click="remove(index)" />
        </div>
      </TarCard>
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

import ContentField from "@/components/shared/ContentField.vue";
import NameField from "@/components/shared/NameField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
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

function setContent(index: number, content: string): void {
  const feature: Feature | undefined = features.value[index];
  if (feature) {
    features.value.splice(index, 1, { ...feature, content });
  }
}
function setName(index: number, name: string): void {
  const feature: Feature | undefined = features.value[index];
  if (feature) {
    features.value.splice(index, 1, { ...feature, name });
  }
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
