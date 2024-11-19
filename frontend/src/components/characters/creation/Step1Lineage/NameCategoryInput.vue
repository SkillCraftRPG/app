<script setup lang="ts">
import { TarButton, TarInput } from "logitar-vue3-ui";
import { ref } from "vue";

const props = defineProps<{
  category: string;
  id?: string;
  names: string[];
}>();

const inputRef = ref<InstanceType<typeof TarInput> | null>(null);
const selected = ref<string>("");

function copyToClipboard(): void {
  if (inputRef.value && selected.value) {
    inputRef.value.focus();
    navigator.clipboard.writeText(selected.value);
  }
}

function select(): void {
  const index: number = Math.floor(Math.random() * props.names.length);
  selected.value = props.names[index];
}
</script>

<template>
  <TarInput :id="id" :model-value="selected" :placeholder="category" readonly ref="inputRef" size="small">
    <template v-if="names.length > 0" #prepend>
      <TarButton icon="fas fa-dice" variant="primary" @click="select" />
    </template>
    <template #append>
      <TarButton icon="fas fa-clipboard" variant="warning" @click="copyToClipboard" />
    </template>
  </TarInput>
</template>
