<template>
  <div class="form-control">
    <label v-if="label" :for="id" class="mb-2">{{ label }}</label>
    <div class="tags">
      <TagInstance
        v-for="(tag, index) in modelValue"
        :key="index"
        :id="`${id}-${index}`"
        :value="tag"
        @removed="onRemove(index)"
        @updated="onUpdate(index, $event)"
      />
      <TagAddNew :id="id" @added="onAdd" />
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import TagAddNew from "./TagAddNew.vue";
import TagInstance from "./TagInstance.vue";

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: string[];
  }>(),
  {
    id: "tags",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: string[]): void;
}>();

function onAdd(tag: string): void {
  tag = tag.trim();
  if (tag) {
    const tags: string[] = [...props.modelValue];
    tags.push(tag);
    emit("update:model-value", tags);
  }
}
function onRemove(index: number): void {
  const tags: string[] = [...props.modelValue];
  tags.splice(index, 1);
  emit("update:model-value", tags);
}
function onUpdate(index: number, tag: string): void {
  tag = tag.trim();
  if (tag) {
    const tags: string[] = [...props.modelValue];
    tags.splice(index, 1, tag);
    emit("update:model-value", tags);
  } else {
    onRemove(index);
  }
}
</script>

<style scoped>
.tags {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 0.5rem;
}
</style>
