<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref, watchEffect } from "vue";
import { nanoid } from "nanoid";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import type { FeaturePayload } from "@/types/castes";

const { t } = useI18n();

const props = defineProps<{
  feature?: FeaturePayload;
}>();

const description = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const hasChanges = computed<boolean>(() => name.value !== (props.feature?.name ?? "") || description.value !== (props.feature?.description ?? ""));
const id = computed<string>(() => (props.feature ? `edit-feature-${props.feature.id ?? nanoid()}` : "create-feature"));

function hide(): void {
  modalRef.value?.hide();
}

function setModel(model?: FeaturePayload): void {
  name.value = model?.name ?? "";
  description.value = model?.description ?? "";
}

const emit = defineEmits<{
  (e: "saved", value: FeaturePayload): void;
}>();

function onCancel(): void {
  setModel(props.feature);
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(() => {
  emit("saved", { id: props.feature?.id, name: name.value, description: description.value });
  onCancel();
});

watchEffect(() => {
  const feature: FeaturePayload | undefined = props.feature;
  setModel(feature);
});
</script>

<template>
  <span>
    <TarButton
      :icon="feature ? 'fas fa-edit' : 'fas fa-plus'"
      :text="t(feature ? 'actions.edit' : 'actions.add')"
      :variant="feature ? 'primary' : 'success'"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}`"
    />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t(feature ? 'castes.features.edit' : 'castes.features.new')">
      <form @submit.prevent="onSubmit">
        <NameInput required v-model="name" />
        <DescriptionTextarea v-model="description" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="feature ? 'fas fa-edit' : 'fas fa-plus'"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(feature ? 'actions.edit' : 'actions.add')"
          :variant="feature ? 'primary' : 'success'"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
