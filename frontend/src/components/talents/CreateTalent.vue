<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import NameInput from "@/components/shared/NameInput.vue";
import TierSelect from "@/components/shared/TierSelect.vue";
import type { TalentModel, CreateOrReplaceTalentPayload } from "@/types/talents";
import { createTalent } from "@/api/talents";

const { t } = useI18n();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");
const tier = ref<number>();

const hasChanges = computed<boolean>(() => Boolean(name.value.trim()));

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: TalentModel): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  tier.value = undefined;
  name.value = "";
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (typeof tier.value === "number") {
    try {
      const payload: CreateOrReplaceTalentPayload = {
        tier: tier.value,
        name: name.value,
        allowMultiplePurchases: false,
      };
      const talent: TalentModel = await createTalent(payload);
      emit("created", talent);
      hide();
    } catch (e: unknown) {
      emit("error", e);
    }
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-talent" />
    <TarModal :close="t('actions.close')" id="create-talent" ref="modalRef" :title="t('talents.create')">
      <form @submit.prevent="onSubmit">
        <TierSelect required v-model="tier" />
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
