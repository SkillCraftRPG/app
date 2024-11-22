<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref, watchEffect } from "vue";
import { nanoid } from "nanoid";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import type { TraitPayload } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  trait?: TraitPayload;
}>();

const description = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const hasChanges = computed<boolean>(() => name.value !== (props.trait?.name ?? "") || description.value !== (props.trait?.description ?? ""));
const id = computed<string>(() => (props.trait ? `edit-trait-${props.trait.id ?? nanoid()}` : "create-trait"));

function hide(): void {
  modalRef.value?.hide();
}

function setModel(model?: TraitPayload): void {
  name.value = model?.name ?? "";
  description.value = model?.description ?? "";
}

const emit = defineEmits<{
  (e: "saved", value: TraitPayload): void;
}>();

function onCancel(): void {
  setModel(props.trait);
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(() => {
  emit("saved", { id: props.trait?.id, name: name.value, description: description.value });
  onCancel();
});

watchEffect(() => {
  const trait: TraitPayload | undefined = props.trait;
  setModel(trait);
});
</script>

<template>
  <span>
    <TarButton
      :icon="trait ? 'fas fa-edit' : 'fas fa-plus'"
      :text="t(trait ? 'actions.edit' : 'actions.add')"
      :variant="trait ? 'primary' : 'success'"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}`"
    />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t(trait ? 'lineages.traits.edit' : 'lineages.traits.new')">
      <form @submit.prevent="onSubmit">
        <NameInput required v-model="name" />
        <DescriptionTextarea v-model="description" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="trait ? 'fas fa-edit' : 'fas fa-plus'"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(trait ? 'actions.edit' : 'actions.add')"
          :variant="trait ? 'primary' : 'success'"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
