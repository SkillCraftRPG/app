<template>
  <div>
    <ul class="nav nav-tabs" :id="`tab_${id}_headers`" role="tablist">
      <li v-for="[key, tab] in tabs" :key="key" class="nav-item" role="presentation">
        <button
          :class="{ 'nav-link': true, active: tab.active }"
          :id="`tab_${tab.id}_head`"
          data-bs-toggle="tab"
          :data-bs-target="`#tab_${tab.id}_pane`"
          type="button"
          role="tab"
          :aria-controls="`${tab.id}_pane`"
          :aria-selected="tab.active ? 'true' : 'false'"
          :disabled="parseBoolean(tab.disabled)"
        >
          {{ tab.title }}
        </button>
      </li>
    </ul>
    <div class="tab-content mt-3" :id="`${id}_contents`">
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { provide, ref } from "vue";
import { parsingUtils } from "logitar-js";

import { type TabContainerOptions, type TabOptions, bindTabKey, unbindTabKey } from "@/types/tar/tabs";

const { parseBoolean } = parsingUtils;

withDefaults(defineProps<TabContainerOptions>(), {
  id: "tabs",
});

const tabs = ref<Map<string, TabOptions>>(new Map<string, TabOptions>());

function bindTab(tab: TabOptions): void {
  if (tab.id) {
    tabs.value.set(tab.id, tab);
  }
}
function unbindTab(tab: TabOptions): void {
  if (tab.id) {
    tabs.value.delete(tab.id);
  }
}
provide(bindTabKey, bindTab);
provide(unbindTabKey, unbindTab);
</script>
