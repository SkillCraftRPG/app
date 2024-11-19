<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import NameInput from "@/components/shared/NameInput.vue";
import type { CreateOrReplaceLineagePayload, LineageModel } from "@/types/lineages";
import { createLineage } from "@/api/lineages";

const { t } = useI18n();

const props = defineProps<{
  species?: LineageModel;
}>();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const hasChanges = computed<boolean>(() => Boolean(name.value.trim()));

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: LineageModel): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  name.value = "";
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: CreateOrReplaceLineagePayload = {
      parentId: props.species?.id,
      name: name.value,
      attributes: { agility: 0, coordination: 0, intellect: 0, presence: 0, sensitivity: 0, spirit: 0, vigor: 0, extra: 0 },
      traits: [],
      languages: { ids: [], extra: 0 },
      names: { family: [], female: [], male: [], unisex: [], custom: [] },
      speeds: { walk: 0, climb: 0, swim: 0, fly: 0, hover: 0, burrow: 0 },
      size: { category: "Medium" },
      weight: {},
      ages: {},
    };
    const lineage: LineageModel = await createLineage(payload);
    emit("created", lineage);
    hide();
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-lineage" />
    <TarModal :close="t('actions.close')" id="create-lineage" ref="modalRef" :title="t(`lineages.create.${species ? 'nation' : 'species'}`)">
      <form @submit.prevent="onSubmit">
        <NameInput required v-model="name" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          icon="fas fa-plus"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.create')"
          variant="success"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
