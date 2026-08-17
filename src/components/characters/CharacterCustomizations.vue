<template>
  <div class="row">
    <div v-for="gift in gifts" :key="gift.id" :class="classes">
      <CharacterCustomization class="mb-3" :customization="gift" />
    </div>
    <div v-for="disability in disabilities" :key="disability.id" :class="classes">
      <CharacterCustomization class="mb-3" :customization="disability" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";

import CharacterCustomization from "./CharacterCustomization.vue";
import type { Customization } from "@/types/customizations";

const { orderBy } = arrayUtils;

const props = defineProps<{
  customizations: Customization[];
}>();

const classes = computed<string>(() => {
  switch (props.customizations.length) {
    case 1:
    case 2:
      return "col-md-6";
    case 5:
    case 6:
      return "col-md-4";
    default:
      return "col-md-6 col-lg-4 col-xl-3";
  }
});
const disabilities = computed<Customization[]>(() =>
  orderBy(
    props.customizations.filter((customization) => customization.kind === "Disability"),
    "name",
  ),
);
const gifts = computed<Customization[]>(() =>
  orderBy(
    props.customizations.filter((customization) => customization.kind === "Gift"),
    "name",
  ),
);
</script>
