<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import NameInput from "@/components/shared/NameInput.vue";
import type { CreateOrReplaceEducationPayload, EducationModel } from "@/types/educations";
import { createEducation } from "@/api/educations";

const { t } = useI18n();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const hasChanges = computed<boolean>(() => Boolean(name.value.trim()));

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: EducationModel): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  name.value = "";
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: CreateOrReplaceEducationPayload = {
      name: name.value,
    };
    const education: EducationModel = await createEducation(payload);
    emit("created", education);
    hide();
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-education" />
    <TarModal :close="t('actions.close')" id="create-education" ref="modalRef" :title="t('educations.create')">
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
