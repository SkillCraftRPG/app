<template>
  <div class="h-100">
    <TarCard class="clickable d-flex flex-column h-100" @click="open">
      <template #contents>
        <div class="card-body text-center d-flex flex-column flex-grow-1 justify-content-center">
          <div class="small text-body-secondary">{{ t("educations.label") }}</div>
          <div class="fw-semibold">{{ education.name }}</div>
          <div v-if="education.feature" class="text-body-secondary">{{ education.feature.name }}</div>
        </div>
      </template>
    </TarCard>
    <EducationDetailModal :education="education" ref="modal" />
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import EducationDetailModal from "@/components/educations/EducationDetailModal.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { Education } from "@/types/educations";

const { t } = useI18n();

defineProps<{
  education: Education;
}>();

const modal = ref<InstanceType<typeof EducationDetailModal> | null>(null);

function open(): void {
  modal.value?.open();
}
</script>
