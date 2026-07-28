<template>
  <TarBreadcrumb :breadcrumbs="breadcrumbs" :divider="divider" />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarBreadcrumb from "@/components/tar/TarBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import { useWorldStore } from "@/stores/world";

const world = useWorldStore();
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    current?: string;
    divider?: string;
    parent?: Breadcrumb | Breadcrumb[];
    root?: boolean | string;
  }>(),
  {
    divider: "›",
  },
);

const isRoot = computed<boolean>(() => parseBoolean(props.root) ?? false);
const breadcrumbs = computed<Breadcrumb[]>(() => {
  const breadcrumbs: Breadcrumb[] = [];
  breadcrumbs.push({ text: t("worlds.title"), to: { name: "Worlds" } });
  if (Array.isArray(props.parent)) {
    props.parent.forEach((breadcrumb) => breadcrumbs.push(breadcrumb));
  } else if (props.parent) {
    breadcrumbs.push(props.parent);
  }
  if (world.current && !isRoot.value) {
    breadcrumbs.push({
      text: world.current.name ?? world.current.key,
      to: { name: "World", params: { id: world.current.id } },
    });
  }
  if (props.current) {
    breadcrumbs.push({ text: props.current });
  }
  return breadcrumbs;
});
</script>
