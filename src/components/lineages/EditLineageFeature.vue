<template>
  <div>
    <TarButton
      :icon="`fas fa-${isEdit ? 'edit' : 'plus'}`"
      :outline="isEdit"
      :size="isEdit ? undefined : 'large'"
      :text="t(`actions.${isEdit ? 'edit' : 'create'}`)"
      @click="open"
    />
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="title">
      <form @submit.prevent="handleSubmit(submit)">
        <NameField class="mb-3" required v-model="name" />
        <ContentField class="mb-3" v-model="content" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="!hasChanges || isLoading"
          :icon="`fas fa-${isEdit ? 'floppy-disk' : 'plus'}`"
          :loading="isLoading"
          :status="t('loading')"
          :text="t(`actions.${isEdit ? 'save' : 'create'}`)"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import ContentField from "@/components/shared/ContentField.vue";
import NameField from "@/components/shared/NameField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Feature } from "@/types/features";
import type { Lineage, LineageFeature } from "@/types/lineages";
import { createLineageFeature, replaceLineageFeature } from "@/api/lineages";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  feature?: LineageFeature;
  lineage: Lineage;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Lineage): void;
}>();

const content = ref<string>("");
const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const hasChanges = computed<boolean>(() => !props.feature || props.feature.name !== name.value || (props.feature.content ?? "") !== content.value);
const isEdit = computed<boolean>(() => Boolean(props.feature));
const title = computed<string>(() => t(`lineages.features.${isEdit.value ? "edit" : "create"}`));

function cancel(): void {
  reset();
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}

const { handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: Feature = {
        name: name.value,
        content: content.value,
      };
      const lineage: Lineage = props.feature
        ? await replaceLineageFeature(props.lineage.id, props.feature.id, payload)
        : await createLineageFeature(props.lineage.id, payload);
      reset();
      modal.value?.hide();
      emit("updated", lineage);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.feature,
  (feature) => {
    name.value = feature?.name ?? "";
    content.value = feature?.content ?? "";
  },
  { deep: true, immediate: true },
);
</script>
