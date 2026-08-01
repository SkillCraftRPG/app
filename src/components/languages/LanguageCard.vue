<template>
  <component :is="component" :title="language.name" v-bind="cardProps">
    <template v-if="language.script" #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary"><font-awesome-icon icon="fas fa-scroll" />&nbsp;{{ language.script.name }}</h6>
    </template>
    <div v-if="language.summary" class="card-text">{{ language.summary }}</div>
    <slot>
      <StatusBlock :actor="language.updatedBy" class="card-text mt-2 small text-secondary" :date="language.updatedOn" relative />
    </slot>
  </component>
</template>

<script setup lang="ts">
import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";
import { computed } from "vue";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { Language } from "@/types/languages";

const props = defineProps<{
  language: Language;
  to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
}>();

const cardProps = computed(() => (props.to ? { to: props.to } : {}));
const component = computed(() => (props.to ? LinkCard : TarCard));
</script>
