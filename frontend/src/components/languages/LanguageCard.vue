<script setup lang="ts">
import { TarButton, TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import LanguageIcon from "./LanguageIcon.vue";
import ScriptIcon from "./ScriptIcon.vue";
import type { LanguageModel } from "@/types/languages";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  language: LanguageModel;
  remove?: boolean | string;
  view?: boolean | string;
}>();

const hasRemove = computed<boolean>(() => parseBoolean(props.remove) ?? false);
const hasView = computed<boolean>(() => parseBoolean(props.view) ?? false);

defineEmits<{
  (e: "removed"): void;
}>();
</script>

<template>
  <TarCard :title="language.name">
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary"><ScriptIcon /> {{ language.script ?? "—" }}</h6>
    </template>
    <div v-if="hasRemove || hasView" class="float-end">
      <RouterLink
        v-if="hasView"
        :class="{ 'btn btn-primary': true, 'me-1': hasRemove }"
        :to="{ name: 'LanguageEdit', params: { id: language.id } }"
        target="_blank"
      >
        <LanguageIcon /> {{ t("actions.view") }}
      </RouterLink>
      <TarButton v-if="hasRemove" :class="{ 'ms-1': hasView }" icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
    </div>
  </TarCard>
</template>
