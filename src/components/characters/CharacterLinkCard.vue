<template>
  <LinkCard :title="character.name" :to="{ name: 'Character', params: { id: character.id } }">
    <StatusBlock :actor="character.updatedBy" class="card-text mt-2 small text-secondary" :date="character.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Character } from "@/types/characters";

defineProps<{
  character: Character;
}>();
</script>
