<script setup lang="ts">
import { TarBadge, TarButton, TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import CustomizationIcon from "./CustomizationIcon.vue";
import type { CustomizationModel } from "@/types/customizations";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  customization: CustomizationModel;
  nature?: boolean | string;
  remove?: boolean | string;
  view?: boolean | string;
}>();

const hasRemove = computed<boolean>(() => parseBoolean(props.remove) ?? false);
const hasView = computed<boolean>(() => parseBoolean(props.view) ?? false);
const isFromNature = computed<boolean>(() => parseBoolean(props.nature) ?? false);

defineEmits<{
  (e: "removed"): void;
}>();
</script>

<template>
  <TarCard>
    <template #title-override>
      <h5 class="card-title" style="display: inline">
        {{ customization.name }} <TarBadge v-if="isFromNature" class="ms-2" pill>{{ t("natures.select.label") }}</TarBadge>
      </h5>
    </template>
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <font-awesome-icon icon="fas fa-tag" /> {{ t(`customizations.type.options.${customization.type}`) }}
      </h6>
    </template>
    <div v-if="hasRemove || hasView" class="float-end">
      <RouterLink
        v-if="hasView"
        :class="{ 'btn btn-primary': true, 'me-1': hasRemove }"
        :to="{ name: 'CustomizationEdit', params: { id: customization.id } }"
        target="_blank"
      >
        <CustomizationIcon /> {{ t("actions.view") }}
      </RouterLink>
      <TarButton v-if="hasRemove" :class="{ 'ms-1': hasView }" icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
    </div>
  </TarCard>
</template>
