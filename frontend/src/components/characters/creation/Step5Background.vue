<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import CasteSelect from "@/components/castes/CasteSelect.vue";
import EducationSelect from "@/components/castes/EducationSelect.vue";
import type { CasteModel } from "@/types/castes";
import type { EducationModel } from "@/types/educations";
import type { Step5 } from "@/types/characters";

const { t } = useI18n();

const caste = ref<CasteModel>();
const education = ref<EducationModel>();

const emit = defineEmits<{
  (e: "back"): void;
  (e: "continue", value: Step5): void;
  (e: "error", value: unknown): void;
}>();

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => {
  if (caste.value && education.value) {
    emit("continue", {
      casteId: caste.value.id,
      educationId: education.value.id,
      startingWealth: { itemId: "", quantity: 0 },
    });
  }
});
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.background") }}</h3>
    <form @submit="onSubmit">
      <div class="row">
        <CasteSelect class="col" :model-value="caste?.id" required @selected="caste = $event" />
        <EducationSelect class="col" :model-value="education?.id" required @selected="education = $event" />
      </div>
      <!-- TODO(fpion): StartingWealth -->
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="$emit('back')" />
      <TarButton class="ms-1" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
