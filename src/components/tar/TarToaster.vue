<template>
  <div :class="classes">
    <TarToast v-for="toast in toasts" :key="toast.id" v-bind="toast" @hidden="$emit('hidden', toast)" />
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";

import TarToast from "./TarToast.vue";
import type { ToastContainerOptions, ToastOptions } from "@/types/tar/toast";

const props = withDefaults(defineProps<ToastContainerOptions>(), {
  horizontalAlignment: "right",
  verticalAlignment: "top",
});

const classes = computed<string[]>(() => {
  const classes = ["toast-container", "position-fixed", "p-3"];
  switch (props.verticalAlignment) {
    case "bottom":
      classes.push("bottom-0");
      break;
    case "middle":
      classes.push("top-50");
      break;
    default:
      classes.push("top-0");
      break;
  }
  switch (props.horizontalAlignment) {
    case "center":
      classes.push("start-50");
      break;
    case "left":
      classes.push("start-0");
      break;
    default:
      classes.push("end-0");
      break;
  }
  return classes;
});

defineEmits<{
  /**
   * The specified toast has been hidden.
   */
  (e: "hidden", toast: ToastOptions): void;
}>();
</script>
