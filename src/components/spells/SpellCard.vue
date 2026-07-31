<template>
  <LinkCard :to="{ name: 'Spell', params: { id: spell.id } }">
    <template #title-override>
      <div class="d-flex flex-wrap align-items-center gap-2">
        <h5 class="card-title">{{ spell.name }}</h5>
        <SpellTierDisplay short :tier="spell.tier" />
      </div>
    </template>
    <div v-if="spell.summary" class="card-text">{{ spell.summary }}</div>
    <StatusBlock :actor="spell.updatedBy" class="card-text mt-2 small text-secondary" :date="spell.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import SpellTierDisplay from "./SpellTierDisplay.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Spell } from "@/types/spells";

defineProps<{
  spell: Spell;
}>();
</script>
