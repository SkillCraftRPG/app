<template>
  <form @submit.prevent="handleSubmit(submit)">
    <h3 class="h5">{{ t("lineages.physical.speeds") }}</h3>
    <!-- TODO(fpion): 5 number inputs + Hover switch -->
    <h3 class="h5">{{ t("lineages.physical.size") }}</h3>
    <div class="row">
      <div class="col-md-6">
        <SizeCategoryField class="mb-3" v-model="sizeCategory" />
      </div>
      <div class="col-md-6">
        <HeightRollField class="mb-3" v-model="height" />
      </div>
    </div>
    <h3 class="h5">{{ t("lineages.physical.weight") }}</h3>
    <!-- TODO(fpion): 5 roll inputs -->
    <h3 class="h5">{{ t("lineages.physical.age") }}</h3>
    <!-- TODO(fpion): 4 number inputs -->
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

import HeightRollField from "@/components/lineages/HeightRollField.vue";
import SizeCategoryField from "@/components/game/SizeCategoryField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Lineage, UpdateLineagePayload } from "@/types/lineages";
import type { SizeCategory } from "@/types/game";
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

const height = ref<string>("");
const isLoading = ref<boolean>(false);
const sizeCategory = ref<SizeCategory>("Medium");

const hasChanges = computed<boolean>(() => props.lineage.size.category !== sizeCategory.value || (props.lineage.size.height ?? "") !== height.value);

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateLineagePayload = {
        size: {
          category: sizeCategory.value,
          height: height.value,
        },
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
    sizeCategory.value = lineage.size.category;
    height.value = lineage.size.height ?? "";
  },
  { deep: true, immediate: true },
);
</script>
