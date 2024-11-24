<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import AspectIcon from "@/components/aspects/AspectIcon.vue";
import type { AspectModel } from "@/types/aspects";

const { t } = useI18n();

defineProps<{
  aspect: AspectModel;
}>();
</script>

<template>
  <AppInput disabled floating id="aspect" label="aspects.select.label" :model-value="aspect.name" validation="server">
    <template #append>
      <RouterLink :to="{ name: 'AspectEdit', params: { id: aspect.id } }" class="btn btn-primary" target="_blank">
        <AspectIcon /> {{ t("actions.view") }}
      </RouterLink>
    </template>
  </AppInput>
</template>
#
