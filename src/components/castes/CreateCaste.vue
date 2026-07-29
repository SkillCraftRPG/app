<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.create')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('castes.create')">
      <form @submit.prevent="handleSubmit(submit)">
        <NameField class="mb-3" required v-model="name" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-plus"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.create')"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import NameField from "@/components/shared/NameField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CreateOrReplaceCastePayload, Caste } from "@/types/castes";
import { createCaste } from "@/api/castes";
import { useForm } from "@/forms";

const { t } = useI18n();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const emit = defineEmits<{
  (e: "created", value: Caste): void;
  (e: "error", value: unknown): void;
}>();

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
      const payload: CreateOrReplaceCastePayload = {
        name: name.value,
      };
      const caste: Caste = await createCaste(payload);
      modal.value?.hide();
      emit("created", caste);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
