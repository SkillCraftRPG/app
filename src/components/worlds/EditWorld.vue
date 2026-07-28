<template>
  <div>
    <TarButton icon="fas fa-edit" size="large" :text="t('actions.edit')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="t('worlds.edit')">
      <form @submit.prevent="handleSubmit(submit)">
        <div class="row">
          <div class="col-lg-6">
            <NameInput class="mb-3" required v-model="name" />
          </div>
          <div class="col-lg-6">
            <KeyInput class="mb-3" required v-model="key" />
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
import KeyInput from "./KeyInput.vue";
import NameInput from "@/components/shared/NameInput.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CreateOrReplaceWorldPayload, World } from "@/types/worlds";
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
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const hasChanges = computed<boolean>(() => {
  const world: World = props.world;
  return (world.name ?? "") !== name.value || world.key !== key.value || (world.htmlContent ?? "") !== htmlContent.value;
});

function cancel(): void {
  reset();
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

const { handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceWorldPayload = {
        key: key.value,
        name: name.value,
        htmlContent: htmlContent.value,
      };
      const world: World = await replaceWorld(props.world.id, payload);
      modal.value?.hide();
      emit("updated", world);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
