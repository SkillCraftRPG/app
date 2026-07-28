<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.create')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('scripts.create')">
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
import type { CreateOrReplaceScriptPayload, Script } from "@/types/scripts";
import { createScript } from "@/api/scripts";
import { useForm } from "@/forms";

const { t } = useI18n();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const emit = defineEmits<{
  (e: "created", value: Script): void;
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
      const payload: CreateOrReplaceScriptPayload = {
        name: name.value,
      };
      const script: Script = await createScript(payload);
      modal.value?.hide();
      emit("created", script);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
