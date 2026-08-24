<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.create')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="t('talents.create')">
      <form @submit.prevent="handleSubmit(submit)">
        <TalentTierField class="mb-3" required v-model="tier" />
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
import TalentTierField from "./TalentTierField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CreateOrReplaceTalentPayload, Talent } from "@/types/talents";
import { createTalent } from "@/api/talents";
import { useForm } from "@/forms";

const { t } = useI18n();

const emit = defineEmits<{
  (e: "created", value: Talent): void;
  (e: "error", value: unknown): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");
const tier = ref<number>();

function cancel(): void {
  reset();
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}

const { handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && typeof tier.value === "number") {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceTalentPayload = {
        tier: tier.value,
        name: name.value,
      };
      const talent: Talent = await createTalent(payload);
      modal.value?.hide();
      emit("created", talent);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
