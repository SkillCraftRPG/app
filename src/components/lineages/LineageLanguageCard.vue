<template>
  <div class="position-relative h-100">
    <LanguageCard class="h-100" :language="language" />
    <div class="position-absolute top-0 end-0 m-2" @click.stop>
      <button type="button" class="btn btn-sm" @click="$emit('remove')">
        <font-awesome-icon icon="fas fa-xmark" aria-hidden="true" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import LanguageCard from "@/components/languages/LanguageCard.vue";
import type { Language } from "@/types/languages";

defineProps<{
  language: Language;
}>();

defineEmits<{
  (e: "remove"): void;
}>();
</script>
