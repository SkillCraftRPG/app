<template>
  <div @keydown.enter.prevent="onConfirm" @keydown.esc.prevent="onCancel">
    <TarInput :id="id" size="small" v-model="tag">
      <template #append>
        <TarButton :disabled="!tag" icon="fas fa-plus" outline size="small" @click="onConfirm" />
      </template>
    </TarInput>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";

import TarButton from "@/components/tar/TarButton.vue";
import TarInput from "@/components/tar/TarInput.vue";

defineProps<{
  id?: string;
}>();

const emit = defineEmits<{
  (e: "added", value: string): void;
}>();

const tag = ref<string>("");

function onCancel(): void {
  tag.value = "";
}
function onConfirm(): void {
  if (tag.value) {
    emit("added", tag.value);
    tag.value = "";
  }
}
</script>
