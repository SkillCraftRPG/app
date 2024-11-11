<script setup lang="ts">
import { useI18n } from "vue-i18n";

import NamesText from "./NamesText.vue";
import type { NamesModel } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  modelValue: NamesModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: NamesModel): void;
}>();

function setText(text?: string): void {
  const names: NamesModel = { ...props.modelValue, text };
  emit("update:model-value", names);
}
</script>

<template>
  <div>
    <h3>{{ t("lineages.names.label") }}</h3>
    <NamesText :model-value="modelValue.text" @update:model-value="setText" />
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("lineages.names.category") }}</th>
          <th scope="col">{{ t("lineages.names.label") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>{{ t("lineages.names.family") }}</td>
          <td>{{ modelValue.family.length > 0 ? modelValue.family.join(", ") : "—" }}</td>
          <td>
            <!-- TODO(fpion): buttons -->
          </td>
        </tr>
        <tr>
          <td>{{ t("lineages.names.female") }}</td>
          <td>{{ modelValue.female.length > 0 ? modelValue.female.join(", ") : "—" }}</td>
          <td>
            <!-- TODO(fpion): buttons -->
          </td>
        </tr>
        <tr>
          <td>{{ t("lineages.names.male") }}</td>
          <td>{{ modelValue.male.length > 0 ? modelValue.male.join(", ") : "—" }}</td>
          <td>
            <!-- TODO(fpion): buttons -->
          </td>
        </tr>
        <tr>
          <td>{{ t("lineages.names.unisex") }}</td>
          <td>{{ modelValue.unisex.length > 0 ? modelValue.unisex.join(", ") : "—" }}</td>
          <td>
            <!-- TODO(fpion): buttons -->
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
