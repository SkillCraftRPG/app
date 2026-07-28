<template>
  <LinkCard :title="language.name" :to="{ name: 'Language', params: { id: language.id } }">
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <font-awesome-icon icon="fas fa-scroll" />&nbsp;<template v-if="language.script">{{ language.script.name }}</template
        ><span v-else class="text-muted">&mdash;</span>
      </h6>
    </template>
    <div class="card-text">
      <span v-if="language.summary">{{ language.summary }}</span>
      <span class="text-muted" v-else>&mdash;</span>
    </div>
    <StatusBlock :actor="language.updatedBy" class="card-text mt-2 small text-secondary" :date="language.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Language } from "@/types/languages";

defineProps<{
  language: Language;
}>();
</script>
