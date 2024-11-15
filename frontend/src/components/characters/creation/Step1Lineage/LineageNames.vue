<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import NameCategoryInput from "./NameCategoryInput.vue";
import type { LineageModel, NamesModel } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  lineage: LineageModel;
}>();

const names = computed<NamesModel>(() => props.lineage.names);
</script>

<template>
  <div>
    <p v-if="names.text">{{ names.text }}</p>
    <table class="table table-striped">
      <tbody>
        <tr v-if="names.family.length > 0">
          <th scope="row">{{ t("lineages.names.family") }}</th>
          <td>
            <NameCategoryInput :category="t('lineages.names.family')" id="family" :names="names.family" />
          </td>
          <td>{{ names.family.join(", ") }}</td>
        </tr>
        <tr v-if="names.female.length > 0">
          <th scope="row">{{ t("lineages.names.female") }}</th>
          <td>
            <NameCategoryInput :category="t('lineages.names.female')" id="female" :names="names.female" />
          </td>
          <td>{{ names.female.join(", ") }}</td>
        </tr>
        <tr v-if="names.male.length > 0">
          <th scope="row">{{ t("lineages.names.male") }}</th>
          <td>
            <NameCategoryInput :category="t('lineages.names.male')" id="male" :names="names.male" />
          </td>
          <td>{{ names.male.join(", ") }}</td>
        </tr>
        <tr v-if="names.unisex.length > 0">
          <th scope="row">{{ t("lineages.names.unisex") }}</th>
          <td>
            <NameCategoryInput :category="t('lineages.names.unisex')" id="unisex" :names="names.unisex" />
          </td>
          <td>{{ names.unisex.join(", ") }}</td>
        </tr>
        <tr v-for="(category, index) in names.custom" :key="index">
          <th scope="row">{{ category.key }}</th>
          <NameCategoryInput :category="category.key" id="unisex" :names="names.unisex" />
          <td>{{ category.values.join(", ") }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
