<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.create')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('worlds.create.lead')">
      <p class="text-secondary">{{ t("worlds.create.help") }}</p>
      <KeyAlreadyUsed v-model="keyAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <NameField class="mb-3" :model-value="name" required @update:model-value="updateName" />
        <KeyField class="mb-3" ref="keyInput" required v-model="key" />
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

import KeyAlreadyUsed from "./KeyAlreadyUsed.vue";
import KeyField from "./KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CreateOrReplaceWorldPayload, World } from "@/types/worlds";
import { ErrorCodes, StatusCodes, type ApiFailure, type ProblemDetails } from "@/types/api";
import { createWorld } from "@/api/worlds";
import { slugify } from "@/utils/string";
import { useForm } from "@/forms";

const { t } = useI18n();

const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const emit = defineEmits<{
  (e: "created", value: World): void;
  (e: "error", value: unknown): void;
}>();

function cancel(): void {
  reset();
  keyAlreadyUsed.value = false;
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}

function updateName(value: string): void {
  name.value = value;
  key.value = slugify(value);
}

const { handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceWorldPayload = {
        key: key.value,
        name: name.value,
      };
      const world: World = await createWorld(payload);
      modal.value?.hide();
      emit("created", world);
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.Conflict) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error && problemDetails.error.code === ErrorCodes.KeyAlreadyUsed) {
          keyAlreadyUsed.value = true;
          keyField.value?.focus();
          return;
        }
      }
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
