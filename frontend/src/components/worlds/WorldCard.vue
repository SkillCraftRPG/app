<script setup lang="ts">
import { TarAvatar, TarCard } from "logitar-vue3-ui";

import type { WorldModel } from "@/types/worlds";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{
  subtitle: boolean;
  world: WorldModel;
}>();
</script>

<template>
  <TarCard :title="world.name ?? world.slug" :subtitle="subtitle ? world.slug : undefined">
    <div class="my-2">
      {{ t("worlds.ownedBy") }}
      <TarAvatar :display-name="world.owner.displayName" :email-address="world.owner.emailAddress" icon="fas fa-user" :url="world.owner.pictureUrl" />
      {{ world.owner.displayName }}
    </div>
    <RouterLink class="btn btn-primary float-end" :to="{ name: 'WorldIndex', params: { slug: world.slug } }">
      <font-awesome-icon icon="fas fa-dungeon" /> {{ t("actions.enter") }}
    </RouterLink>
  </TarCard>
</template>
