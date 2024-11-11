<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref, watchEffect } from "vue";
import { parsingUtils } from "logitar-js";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import NameCategoryKeyInput from "./NameCategoryKeyInput.vue";
import NamesTextarea from "./NamesTextarea.vue";
import type { NameCategory } from "@/types/lineages";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    category?: NameCategory;
    custom?: boolean | string;
    id?: string;
  }>(),
  {
    id: "new-category",
  },
);

const key = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const names = ref<string>("");
const namesReference = ref<string>("");

const hasChanges = computed<boolean>(() => key.value !== (props.category?.key ?? "") || names.value !== namesReference.value);
const isCustom = computed<boolean>(() => parseBoolean(props.custom) ?? false);

function hide(): void {
  modalRef.value?.hide();
}

function setModel(model?: NameCategory): void {
  key.value = model?.key ?? "";
  names.value = model?.values.join(", ") ?? "";
  namesReference.value = model?.values.join(", ") ?? "";
}

const emit = defineEmits<{
  (e: "saved", value: NameCategory): void;
}>();

function onCancel(): void {
  setModel(props.category);
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(() => {
  emit("saved", {
    key: key.value,
    values: names.value
      .split(",")
      .map((name) => name.trim())
      .filter((name) => name.length > 0),
  });
  onCancel();
});

watchEffect(() => {
  const category: NameCategory | undefined = props.category;
  setModel(category);
});
</script>

<template>
  <span>
    <TarButton
      :icon="category ? 'fas fa-edit' : 'fas fa-plus'"
      :text="t(category ? 'actions.edit' : 'actions.add')"
      :variant="category ? 'primary' : 'success'"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}-modal`"
    />
    <TarModal
      :close="t('actions.close')"
      :id="`${id}-modal`"
      ref="modalRef"
      :title="t(category ? 'lineages.names.category.edit' : 'lineages.names.category.new')"
    >
      <form @submit.prevent="onSubmit">
        <NameCategoryKeyInput :disabled="!isCustom" :id="`${id}-category`" :required="isCustom" v-model="key" />
        <NamesTextarea :id="`${id}-values`" v-model="names" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="category ? 'fas fa-edit' : 'fas fa-plus'"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(category ? 'actions.edit' : 'actions.add')"
          :variant="category ? 'primary' : 'success'"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
