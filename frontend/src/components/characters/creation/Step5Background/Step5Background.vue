<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import CasteDetail from "./CasteDetail.vue";
import CasteSelect from "@/components/castes/CasteSelect.vue";
import EducationSelect from "@/components/castes/EducationSelect.vue";
import MarkdownText from "@/components/shared/MarkdownText.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
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
      <h5>{{ t("castes.select.label") }}</h5>
      <div class="row">
        <CasteSelect class="col" :model-value="caste?.id" required @selected="caste = $event" />
        <SkillSelect v-if="caste?.skill" class="col" disabled id="caste-skill" :model-value="caste?.skill" validation="server" />
      </div>
      <template v-if="caste">
        <MarkdownText v-if="caste.description" :text="caste.description" />
        <CasteDetail :caste="caste" />
      </template>
      <h5>{{ t("educations.select.label") }}</h5>
      <div class="row">
        <EducationSelect class="col" :model-value="education?.id" required @selected="education = $event" />
        <SkillSelect v-if="education?.skill" class="col" disabled id="education-skill" :model-value="education?.skill" validation="server" />
      </div>
      <MarkdownText v-if="education?.description" :text="education.description" />
      <template v-if="caste && education">
        <h5>{{ t("game.startingWealth") }}</h5>
        <!-- TODO(fpion): StartingWealth -->
      </template>
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="$emit('back')" />
      <TarButton class="ms-1" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
