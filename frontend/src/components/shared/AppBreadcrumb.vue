<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import type { Breadcrumb } from "@/types/components";
import type { WorldModel } from "@/types/worlds";
import { getWorldSlug } from "@/helpers/routingUtils";
import { useWorldStore } from "@/stores/world";

const route = useRoute();
const worldStore = useWorldStore();
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    ariaLabel?: string;
    current: string;
    divider?: string;
    parent?: Breadcrumb;
    world?: WorldModel;
  }>(),
  {
    divider: "›",
  },
);

const currentWorld = ref<WorldModel>();

const breadcrumbs = computed<Breadcrumb[]>(() => {
  const breadcrumbs: Breadcrumb[] = [
    {
      route: { name: "WorldList" },
      text: t("worlds.gateway"),
    },
  ];
  if (route.name !== "WorldIndex" && currentWorld.value) {
    breadcrumbs.push({
      route: { name: "WorldIndex", params: { slug: currentWorld.value.slug } },
      text: currentWorld.value.name ?? currentWorld.value.slug,
    });
  }
  if (props.parent) {
    breadcrumbs.push(props.parent);
  }
  breadcrumbs.push({ text: props.current });
  return breadcrumbs;
});

function getAriaCurrent(breadcrumb: Breadcrumb): "page" | undefined {
  return breadcrumb.route ? undefined : "page";
}
function getClasses(breadcrumb: Breadcrumb): string[] {
  const classes = ["breadcrumb-item"];
  if (!breadcrumb.route) {
    classes.push("active");
  }
  return classes;
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

watch(
  () => props.world,
  async (world) => {
    if (world) {
      currentWorld.value = world;
    } else {
      try {
        const slug: string | undefined = getWorldSlug();
        if (slug) {
          currentWorld.value = await worldStore.retrieve(slug);
        }
      } catch (e: unknown) {
        emit("error", e);
      }
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <nav :aria-label="ariaLabel ? t(ariaLabel) : undefined" :style="divider ? { '--bs-breadcrumb-divider': `'${divider}'` } : undefined">
    <ol class="breadcrumb">
      <li v-for="(breadcrumb, index) in breadcrumbs" :key="index" :class="getClasses(breadcrumb)" :aria-current="getAriaCurrent(breadcrumb)">
        <RouterLink v-if="breadcrumb.route" :to="breadcrumb.route">{{ breadcrumb.text }}</RouterLink>
        <template v-else>{{ breadcrumb.text }}</template>
      </li>
    </ol>
  </nav>
</template>
