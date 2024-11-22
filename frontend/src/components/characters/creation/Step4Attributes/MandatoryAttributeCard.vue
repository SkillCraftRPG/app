<script setup lang="ts">
import { TarButton, TarCard } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { MandatoryAttribute } from "@/types/aspects";

const { t } = useI18n();

defineProps<MandatoryAttribute>();

defineEmits<{
  (e: "best"): void;
  (e: "mandatory"): void;
  (e: "worst"): void;
}>();
</script>

<template>
  <TarCard :title="text">
    <h6 class="card-subtitle mb-2 text-body-secondary"><font-awesome-icon icon="fas fa-tag" /> {{ t(`characters.attributes.mandatory.${selected}`) }}</h6>
    <TarButton v-if="selected !== 'worst'" class="me-1" icon="fas fa-minus" :text="t('characters.attributes.worst.label')" @click="$emit('worst')" />
    <TarButton
      v-if="selected !== 'mandatory'"
      :class="{ 'me-1': selected !== 'best', 'ms-1': selected !== 'worst' }"
      icon="fas fa-equals"
      :text="t('characters.attributes.mandatory.label')"
      @click="$emit('mandatory')"
    />
    <TarButton v-if="selected !== 'best'" class="ms-1" icon="fas fa-plus" :text="t('characters.attributes.best.label')" @click="$emit('best')" />
  </TarCard>
</template>
