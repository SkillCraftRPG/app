<template>
  <div>
    <div v-if="isEditing" @keydown.enter.prevent="onConfirm" @keydown.esc.prevent="onCancel">
      <TarInput :id="id" ref="inputRef" validation="server" v-model="tag">
        <template #append>
          <TarButton icon="fas fa-ban" variant="secondary" @click="onCancel" />
          <TarButton :disabled="!tag" icon="fas fa-save" />
        </template>
      </TarInput>
    </div>
    <div v-else class="btn-group" role="group" aria-label="Tag Actions">
      <TarButton :text="value" @click="onEdit" />
      <TarButton icon="fas fa-times" @click="$emit('removed')" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { nextTick, ref } from "vue";

import TarButton from "@/components/tar/TarButton.vue";
import TarInput from "@/components/tar/TarInput.vue";

const props = defineProps<{
  id?: string;
  value: string;
}>();

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "updated", value: string): void;
}>();

const isEditing = ref<boolean>(false);
const tag = ref<string>("");

const inputRef = ref<InstanceType<typeof TarInput> | null>();

function onCancel(): void {
  isEditing.value = false;
}
function onConfirm(): void {
  if (tag.value) {
    emit("updated", tag.value);
    isEditing.value = false;
  }
}
function onEdit(): void {
  tag.value = props.value;
  isEditing.value = true;
  nextTick(() => inputRef.value?.focus());
}
</script>
