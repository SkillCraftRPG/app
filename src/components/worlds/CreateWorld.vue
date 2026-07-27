<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.create')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('worlds.create.lead')">
      <p class="text-secondary">{{ t("worlds.create.help") }}</p>
      <form @submit.prevent="handleSubmit(submit)">
        <NameInput class="mb-3" :model-value="name" required @update:model-value="updateName" />
        <KeyInput class="mb-3" required v-model="key" />
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

import KeyInput from "./KeyInput.vue";
import NameInput from "@/components/shared/NameInput.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CreateOrReplaceWorldPayload, World } from "@/types/worlds";
import { createWorld } from "@/api/worlds";
import { slugify } from "@/utils/string";
import { useForm } from "@/forms";

const { t } = useI18n();

const isLoading = ref<boolean>(false);
const key = ref<string>("");
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const emit = defineEmits<{
  (e: "created", value: World): void;
  (e: "error", value: unknown): void;
}>();

function cancel(): void {
  reset();
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
    try {
      const payload: CreateOrReplaceWorldPayload = {
        key: key.value,
        name: name.value,
      };
      const world: World = await createWorld(payload);
      modal.value?.hide();
      emit("created", world);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
