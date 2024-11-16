<script setup lang="ts">
import { computed } from "vue";
import { marked } from "marked";
import { parsingUtils } from "logitar-js";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  inline?: boolean | string;
  text: string;
}>();

const isInline = computed<boolean>(() => parseBoolean(props.inline) ?? false);
const html = computed<string>(() => (isInline.value ? marked.parseInline(props.text) : marked.parse(props.text)) as string);
</script>

<template>
  <span v-if="isInline" v-html="html"></span>
  <div v-else v-html="html"></div>
</template>
