<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { SearchResults } from "@/types/search";
import { listPlayers } from "@/api/characters";

defineProps<{
  modelValue?: string;
}>();

const players = ref<string[]>([]);

const options = computed<SelectOption[]>(() => players.value.map((player) => ({ text: player, value: player })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value?: string): void;
}>();

onMounted(async () => {
  try {
    const results: SearchResults<string> = await listPlayers();
    players.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <AppSelect
    floating
    id="script"
    label="characters.player.label"
    :model-value="modelValue"
    :options="options"
    placeholder="characters.player.all"
    validation="server"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
