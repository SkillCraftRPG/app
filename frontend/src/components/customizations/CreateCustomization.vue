<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import CustomizationTypeSelect from "./CustomizationTypeSelect.vue";
import NameInput from "@/components/shared/NameInput.vue";
import type { CustomizationModel, CreateOrReplaceCustomizationPayload, CustomizationType } from "@/types/customizations";
import { createCustomization } from "@/api/customizations";

const { t } = useI18n();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");
const type = ref<CustomizationType>();

const hasChanges = computed<boolean>(() => Boolean(name.value.trim()));

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: CustomizationModel): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  name.value = "";
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (type.value) {
    try {
      const payload: CreateOrReplaceCustomizationPayload = {
        type: type.value,
        name: name.value,
      };
      const customization: CustomizationModel = await createCustomization(payload);
      emit("created", customization);
      hide();
    } catch (e: unknown) {
      emit("error", e);
    }
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-customization" />
    <TarModal :close="t('actions.close')" id="create-customization" ref="modalRef" :title="t('customizations.create')">
      <form @submit.prevent="onSubmit">
        <CustomizationTypeSelect required v-model="type" />
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
