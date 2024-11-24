<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { AspectModel } from "@/types/aspects";
import type { Attribute } from "@/types/game";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

type TranslatedAttribute = {
  attribute: Attribute;
  text: string;
};

const props = defineProps<{
  aspect: AspectModel;
  optional?: boolean | string;
}>();

const attributes = computed<TranslatedAttribute[]>(() => {
  const attributes: TranslatedAttribute[] = [];
  if (parseBoolean(props.optional)) {
    if (props.aspect.attributes.optional1) {
      attributes.push({ attribute: props.aspect.attributes.optional1, text: t(`game.attribute.options.${props.aspect.attributes.optional1}`) });
    }
    if (props.aspect.attributes.optional2) {
      attributes.push({ attribute: props.aspect.attributes.optional2, text: t(`game.attribute.options.${props.aspect.attributes.optional2}`) });
    }
  } else {
    if (props.aspect.attributes.mandatory1) {
      attributes.push({ attribute: props.aspect.attributes.mandatory1, text: t(`game.attribute.options.${props.aspect.attributes.mandatory1}`) });
    }
    if (props.aspect.attributes.mandatory2) {
      attributes.push({ attribute: props.aspect.attributes.mandatory2, text: t(`game.attribute.options.${props.aspect.attributes.mandatory2}`) });
    }
  }
  return orderBy(attributes, "text");
});
</script>

<template>
  <div>
    <template v-if="attributes.length === 2">
      {{ attributes[0].text }}
      <br />
      {{ attributes[1].text }}
    </template>
    <template v-else-if="attributes.length === 1">{{ attributes[0].text }}</template>
    <template v-else>{{ "—" }}</template>
  </div>
</template>
