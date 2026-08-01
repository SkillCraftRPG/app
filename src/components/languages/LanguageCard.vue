<template>
  <LinkableCard :title="language.name" :to="to">
    <template v-if="language.script" #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary"><font-awesome-icon icon="fas fa-scroll" />&nbsp;{{ language.script.name }}</h6>
    </template>
    <div v-if="language.summary" class="card-text">{{ language.summary }}</div>
    <slot>
      <StatusBlock :actor="language.updatedBy" class="card-text mt-2 small text-secondary" :date="language.updatedOn" relative />
    </slot>
  </LinkableCard>
</template>

<script setup lang="ts">
import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";

import LinkableCard from "@/components/shared/LinkableCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Language } from "@/types/languages";

defineProps<{
  language: Language;
  to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
}>();
</script>
