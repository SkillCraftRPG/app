<script setup lang="ts">
import { onMounted, ref } from "vue";
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

const breadcrumbs = ref<Breadcrumb[]>([]);

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

onMounted(async () => {
  try {
    breadcrumbs.value.push({
      route: { name: "WorldList" },
      text: t("worlds.gateway"),
    });
    if (route.name !== "WorldIndex") {
      let world: WorldModel | undefined = props.world;
      console.log(world);
      if (!world) {
        const slug: string | undefined = getWorldSlug();
        if (slug) {
          const world: WorldModel = await worldStore.retrieve(slug);
          breadcrumbs.value.push({
            route: { name: "WorldIndex", params: { slug: world.slug } },
            text: world.name ?? world.slug,
          });
        }
      }
      console.log(world);
      if (world) {
        breadcrumbs.value.push({
          route: { name: "WorldIndex", params: { slug: world.slug } },
          text: world.name ?? world.slug,
        });
      }
    }
    if (props.parent) {
      breadcrumbs.value.push(props.parent);
    }
    breadcrumbs.value.push({ text: props.current });
  } catch (e: unknown) {
    emit("error", e);
  }
});
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
