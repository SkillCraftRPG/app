<template>
  <TarCard>
    <div class="d-flex flex-column flex-sm-row align-items-stretch align-items-sm-center justify-content-between gap-3">
      <h5 class="card-title mb-0">{{ title }}</h5>
      <TarButton icon="fas fa-dungeon" outline :text="t('actions.enter')" @click="enter" />
    </div>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { World } from "@/types/worlds";

const router = useRouter();
const { t } = useI18n();

const props = defineProps<{
  world: World;
}>();

const title = computed<string>(() => props.world.name ?? props.world.key);

function enter(): void {
  router.push({ name: "World", params: { id: props.world.id } });
}
</script>
