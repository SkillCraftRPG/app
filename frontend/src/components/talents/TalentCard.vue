<script setup lang="ts">
import { TarButton, TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import type { TalentModel } from "@/types/talents";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  remove?: boolean | string;
  talent: TalentModel;
  view?: boolean | string;
}>();

const hasRemove = computed<boolean>(() => parseBoolean(props.remove) ?? false);
const hasView = computed<boolean>(() => parseBoolean(props.view) ?? false);

defineEmits<{
  (e: "removed"): void;
}>();
</script>

<template>
  <TarCard :title="talent.name">
    <div v-if="hasRemove || hasView" class="float-end">
      <RouterLink
        v-if="hasView"
        :class="{ 'btn btn-primary': true, 'me-1': hasRemove }"
        :to="{ name: 'TalentEdit', params: { id: talent.id } }"
        target="_blank"
      >
        <font-awesome-icon icon="fas fa-eye" /> {{ t("actions.view") }}
      </RouterLink>
      <TarButton v-if="hasRemove" :class="{ 'ms-1': hasView }" icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
    </div>
  </TarCard>
</template>
