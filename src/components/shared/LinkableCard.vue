<template>
  <RouterLink v-if="to" class="card clickable" :to="to">
    <slot name="header"></slot>
    <slot name="image-top">
      <TarImage v-if="topImage" class="card-img-top" v-bind="topImage" />
    </slot>
    <slot name="contents">
      <div class="card-body">
        <slot name="title-override">
          <h5 v-if="title" class="card-title">{{ title }}</h5>
        </slot>
        <slot name="subtitle-override">
          <h6 v-if="subtitle" class="card-subtitle mb-2 text-body-secondary">{{ subtitle }}</h6>
        </slot>
        <slot></slot>
      </div>
    </slot>
    <slot name="image-bottom">
      <TarImage v-if="bottomImage" class="card-img-bottom" v-bind="bottomImage" />
    </slot>
    <slot name="footer"></slot>
  </RouterLink>
  <TarCard v-else v-bind="props" :class="classes">
    <template #header>
      <slot name="header"></slot>
    </template>
    <template #image-top>
      <slot name="image-top"></slot>
    </template>
    <template #contents>
      <slot name="contents"></slot>
    </template>
    <template #title-override>
      <slot name="title-override"></slot>
    </template>
    <template #subtitle-override>
      <slot name="subtitle-override"></slot>
    </template>
    <slot></slot>
    <template #image-bottom>
      <slot name="image-bottom"></slot>
    </template>
    <template #footer>
      <slot name="footer"></slot>
    </template>
  </TarCard>
</template>

<script setup lang="ts">
import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import TarCard from "@/components/tar/TarCard.vue";
import TarImage from "@/components/tar/TarImage.vue";
import type { CardOptions } from "@/types/tar/card";

const { parseBoolean } = parsingUtils;

const props = defineProps<
  CardOptions & {
    clickable?: boolean | string;
    selected?: boolean | string;
    to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
  }
>();

const classes = computed<string[]>(() => {
  const classes: string[] = [];
  if (parseBoolean(props.clickable)) {
    classes.push("clickable");
  }
  if (parseBoolean(props.selected)) {
    classes.push("border-primary", "bg-primary-subtle");
  }
  return classes;
});
</script>
