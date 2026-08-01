<template>
  <div :class="classes" :id="`tab_${id}_pane`" role="tabpanel" :aria-labelledby="`tab_${id}_head`" tabindex="0">
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, onUnmounted, onUpdated } from "vue";
import { nanoid } from "nanoid";

import { bindTabKey, unbindTabKey, type TabOptions } from "@/types/tar/tabs";

const bindTab: ((tab: TabOptions) => void) | undefined = inject(bindTabKey);
const unbindTab: ((tab: TabOptions) => void) | undefined = inject(unbindTabKey);

const props = withDefaults(defineProps<TabOptions>(), {
  id: () => nanoid(),
});

const classes = computed<string[]>(() => {
  const classes = ["tab-pane", "fade"];
  if (props.active) {
    classes.push("show");
    classes.push("active");
  }
  return classes;
});

onMounted(() => {
  if (bindTab) {
    bindTab(props);
  }
});
onUnmounted(() => {
  if (unbindTab) {
    unbindTab(props);
  }
});
onUpdated(() => {
  if (bindTab) {
    bindTab(props);
  }
});
</script>
