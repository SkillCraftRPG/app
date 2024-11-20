<script setup lang="ts">
import { TarAvatar, TarButton, TarCard } from "logitar-vue3-ui";

import type { WorldModel } from "@/types/worlds";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{
  subtitle: boolean;
  world: WorldModel;
}>();

defineEmits<{
  (e: "entered"): void;
}>();
</script>

<template>
  <TarCard class="card" :title="world.name ?? world.slug" :subtitle="subtitle ? world.slug : undefined" @click="$emit('entered')">
    <div class="my-2">
      {{ t("worlds.ownedBy") }}
      <TarAvatar :display-name="world.owner.displayName" :email-address="world.owner.emailAddress" icon="fas fa-user" :url="world.owner.pictureUrl" />
      {{ world.owner.displayName }}
    </div>
    <TarButton class="float-end" icon="fas fa-dungeon" :text="t('actions.enter')" />
  </TarCard>
</template>

<style scoped>
.card:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
}
</style>
