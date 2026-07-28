<template>
  <div class="d-flex flex-column justify-content-center align-items-center">
    <div class="grid">
      <RouterLink v-for="(tile, index) in tiles" :key="index" :to="tile.to" class="tile">
        <font-awesome-icon class="icon" :icon="tile.icon" /> {{ tile.text }}
      </RouterLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { RouteLocationAsRelativeGeneric } from "vue-router";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

type Tile = {
  icon: string;
  text: string;
  to: RouteLocationAsRelativeGeneric;
};
const tiles = computed<Tile[]>(() => [
  {
    icon: "fas fa-wheelchair",
    text: t("customizations.title"),
    to: { name: "Customizations" },
  },
  {
    icon: "fas fa-scroll",
    text: t("scripts.title"),
    to: { name: "Scripts" },
  },
  {
    icon: "fas fa-language",
    text: t("languages.title"),
    to: { name: "Languages" },
  },
]);
</script>

<style scoped>
.grid {
  display: grid;
  grid-template-columns: repeat(var(--columns), var(--column-width));
  gap: var(--gap);
  max-width: calc(var(--columns) * var(--column-width) + (var(--columns) - 1) * var(--gap));
  margin-bottom: var(--gap);
  --columns: 1;
  --gap: 1.5rem;
  --column-width: 13.5rem;
  --column-height: 13.5rem;
}

.tile {
  box-shadow: rgba(99, 99, 99, 0.2) 0px 2px 8px 0px;
  background-color: var(--bs-tertiary-bg);
  border: 1px solid var(--bs-border-color);
  border-radius: 0.75rem;
  width: 100%;
  max-width: var(--column-width);
  height: var(--column-height);
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  font-size: 1.5rem;
  text-decoration: none;
  gap: 0.5rem;
}
.tile:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
}
.tile .icon {
  font-size: 4.5rem;
}

@media (min-width: 576px) {
  .grid {
    --columns: 2;
  }
}

@media (min-width: 768px) {
  .grid {
    --columns: 3;
  }
}

@media (min-width: 992px) {
  .grid {
    --columns: 4;
  }
}
</style>
