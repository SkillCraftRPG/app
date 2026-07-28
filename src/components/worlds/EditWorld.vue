<template>
  <div>
    <TarButton icon="fas fa-edit" size="large" :text="t('actions.edit')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="t('worlds.edit')">
      <form @submit.prevent="handleSubmit(submit)">
        <KeyAlreadyUsed v-model="keyAlreadyUsed" />
        <div class="row">
          <div class="col-lg-6">
            <NameInput class="mb-3" required v-model="name" />
          </div>
          <div class="col-lg-6">
            <KeyInput class="mb-3" ref="keyInput" required v-model="key" />
          </div>
        </div>
        <ContentTextarea class="mb-3" v-model="htmlContent" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="!hasChanges || isLoading"
          icon="fas fa-floppy-disk"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.save')"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import ContentTextarea from "@/components/shared/ContentTextarea.vue";
import KeyAlreadyUsed from "./KeyAlreadyUsed.vue";
import KeyInput from "./KeyInput.vue";
import NameInput from "@/components/shared/NameInput.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CreateOrReplaceWorldPayload, World } from "@/types/worlds";
import { ErrorCodes, StatusCodes, type ApiFailure, type ProblemDetails } from "@/types/api";
import { replaceWorld } from "@/api/worlds";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  world: World;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: World): void;
}>();

const htmlContent = ref<string>("");
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyInput = ref<InstanceType<typeof KeyInput> | null>(null);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const hasChanges = computed<boolean>(() => {
  const world: World = props.world;
  return (world.name ?? "") !== name.value || world.key !== key.value || (world.htmlContent ?? "") !== htmlContent.value;
});

function cancel(): void {
  reset();
  keyAlreadyUsed.value = false;
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}

watch(
  () => props.world,
  (world) => {
    name.value = world.name ?? "";
    key.value = world.key;
    htmlContent.value = world.htmlContent ?? "";
  },
  { deep: true, immediate: true },
);

const { handleSubmit, reinitialize, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceWorldPayload = {
        key: key.value,
        name: name.value,
        htmlContent: htmlContent.value,
      };
      const world: World = await replaceWorld(props.world.id, payload);
      reinitialize();
      modal.value?.hide();
      emit("updated", world);
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.Conflict) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error && problemDetails.error.code === ErrorCodes.KeyAlreadyUsed) {
          keyAlreadyUsed.value = true;
          keyInput.value?.focus();
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
