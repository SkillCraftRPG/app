<template>
  <section>
    <h2 class="h3">{{ t("characters.creation.context.title") }}</h2>
    <section v-if="isLoading < 2">
      <h3 class="h5">{{ t("castes.label") }}</h3>
      <template v-if="castes.length">
        <p class="text-body-secondary">{{ t("characters.creation.context.caste.help") }}</p>
        <div class="row">
          <div v-for="caste in castes" :key="caste.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <CasteCard
              :caste="caste"
              class="d-flex flex-column h-100"
              clickable
              :selected="caste.id === selectedCaste?.id"
              selection="single"
              @click="toggleCaste(caste)"
            />
          </div>
        </div>
      </template>
      <TarAlert v-else class="d-flex justify-content-between" show variant="warning">
        <div>
          <strong>{{ t("characters.creation.context.caste.empty.lead") }}</strong> {{ t("characters.creation.context.caste.empty.help") }}
        </div>
        <RouterLink :to="{ name: 'Castes' }" class="btn btn-primary">
          <font-awesome-icon aria-hidden="true" icon="fas fa-screwdriver-wrench" />&nbsp;{{ t("castes.title") }}
        </RouterLink>
      </TarAlert>
    </section>
    <section v-if="isLoading < 1">
      <h3 class="h5">{{ t("educations.label") }}</h3>
      <template v-if="educations.length">
        <p class="text-body-secondary">{{ t("characters.creation.context.education.help") }}</p>
        <div class="row">
          <div v-for="education in educations" :key="education.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <EducationCard
              class="d-flex flex-column h-100"
              clickable
              :education="education"
              :selected="education.id === selectedEducation?.id"
              selection="single"
              @click="toggleEducation(education)"
            />
          </div>
        </div>
      </template>
      <TarAlert v-else class="d-flex justify-content-between" show variant="warning">
        <div>
          <strong>{{ t("characters.creation.context.education.empty.lead") }}</strong> {{ t("characters.creation.context.education.empty.help") }}
        </div>
        <RouterLink :to="{ name: 'Educations' }" class="btn btn-primary">
          <font-awesome-icon aria-hidden="true" icon="fas fa-graduation-cap" />&nbsp;{{ t("educations.title") }}
        </RouterLink>
      </TarAlert>
    </section>
    <LoadingSpinner v-if="isLoading" />
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton :disabled="!canSubmit" icon="fas fa-arrow-right" :text="t('actions.next')" @click="submit" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Caste } from "@/types/castes";
import type { Education } from "@/types/educations";
import type { SearchResults } from "@/types/search";
import { listCastes } from "@/api/castes";
import { listEducations } from "@/api/educations";
import { useCharacterStore } from "@/stores/character";
import CasteCard from "@/components/castes/CasteCard.vue";
import EducationCard from "@/components/educations/EducationCard.vue";

const character = useCharacterStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const emit = defineEmits<{
  (e: "abandon"): void;
  (e: "error", value: unknown): void;
}>();

const castes = ref<Caste[]>([]);
const educations = ref<Education[]>([]);
const isLoading = ref<number>(2);
const selectedCaste = ref<Caste>();
const selectedEducation = ref<Education>();

const canSubmit = computed<boolean>(() => Boolean(selectedCaste.value && selectedEducation.value));

function toggleCaste(caste: Caste): void {
  if (selectedCaste.value?.id === caste.id) {
    selectedCaste.value = undefined;
  } else {
    selectedCaste.value = caste;
  }
}

function toggleEducation(education: Education): void {
  if (selectedEducation.value?.id === education.id) {
    selectedEducation.value = undefined;
  } else {
    selectedEducation.value = education;
  }
}

function submit(): void {
  if (canSubmit.value && selectedCaste.value && selectedEducation.value) {
    character.saveContext(selectedCaste.value, selectedEducation.value);
  }
}

onMounted(async () => {
  try {
    const casteResults: SearchResults<Caste> = await listCastes();
    castes.value = orderBy(casteResults.items, "name");
    selectedCaste.value = castes.value.find((caste) => caste.id === character.creation.caste?.id);

    isLoading.value--;

    const educationResults: SearchResults<Education> = await listEducations();
    educations.value = orderBy(educationResults.items, "name");
    selectedEducation.value = educations.value.find((education) => education.id === character.creation.education?.id);
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = 0;
  }
});
</script>
