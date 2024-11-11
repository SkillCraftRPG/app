<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import NameCategoryEdit from "./NameCategoryEdit.vue";
import NamesText from "./NamesText.vue";
import type { NameCategory, NamesModel } from "@/types/lineages";

const { t } = useI18n();

type NameKey = "family" | "female" | "male" | "unisex";

const props = defineProps<{
  modelValue: NamesModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: NamesModel): void;
}>();

function addCategory(category: NameCategory): void {
  const names: NamesModel = { ...props.modelValue, custom: [...props.modelValue.custom, category] };
  emit("update:model-value", names);
}
function removeCategory(index: number): void {
  const names: NamesModel = { ...props.modelValue, custom: [...props.modelValue.custom] };
  names.custom.splice(index, 1);
  emit("update:model-value", names);
}
function updateCategory(key: number | NameKey, category: NameCategory): void {
  console.log(key);
  console.log(category);
  const names: NamesModel = { ...props.modelValue };
  if (typeof key === "number") {
    names.custom = [...names.custom];
    names.custom.splice(key, 1, category);
  } else {
    switch (key) {
      case "family":
        names.family = [...category.values];
        break;
      case "female":
        names.female = [...category.values];
        break;
      case "male":
        names.male = [...category.values];
        break;
      case "unisex":
        names.unisex = [...category.values];
        break;
      default:
        throw new Error(`The name category "${key}" is not supported.`);
    }
  }
  emit("update:model-value", names);
}

function setText(text?: string): void {
  const names: NamesModel = { ...props.modelValue, text };
  emit("update:model-value", names);
}
</script>

<template>
  <div>
    <h3>{{ t("lineages.names.label") }}</h3>
    <NamesText :model-value="modelValue.text" @update:model-value="setText" />
    <div class="mb-3">
      <NameCategoryEdit custom @saved="addCategory" />
    </div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("lineages.names.category.label") }}</th>
          <th scope="col">{{ t("lineages.names.label") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>{{ t("lineages.names.family") }}</td>
          <td>{{ modelValue.family.length > 0 ? modelValue.family.join(", ") : "—" }}</td>
          <td>
            <NameCategoryEdit
              :category="{ key: t('lineages.names.family'), values: modelValue.family }"
              id="family"
              @saved="updateCategory('family', $event)"
            />
          </td>
        </tr>
        <tr>
          <td>{{ t("lineages.names.female") }}</td>
          <td>{{ modelValue.female.length > 0 ? modelValue.female.join(", ") : "—" }}</td>
          <td>
            <NameCategoryEdit
              :category="{ key: t('lineages.names.female'), values: modelValue.female }"
              id="female"
              @saved="updateCategory('female', $event)"
            />
          </td>
        </tr>
        <tr>
          <td>{{ t("lineages.names.male") }}</td>
          <td>{{ modelValue.male.length > 0 ? modelValue.male.join(", ") : "—" }}</td>
          <td>
            <NameCategoryEdit :category="{ key: t('lineages.names.male'), values: modelValue.male }" id="male" @saved="updateCategory('male', $event)" />
          </td>
        </tr>
        <tr>
          <td>{{ t("lineages.names.unisex") }}</td>
          <td>{{ modelValue.unisex.length > 0 ? modelValue.unisex.join(", ") : "—" }}</td>
          <td>
            <NameCategoryEdit
              :category="{ key: t('lineages.names.unisex'), values: modelValue.unisex }"
              id="unisex"
              @saved="updateCategory('unisex', $event)"
            />
          </td>
        </tr>
        <tr v-for="(custom, index) in modelValue.custom" :key="index">
          <td>{{ custom.key }}</td>
          <td>{{ custom.values.length > 0 ? custom.values.join(", ") : "—" }}</td>
          <td>
            <NameCategoryEdit class="me-1" :category="custom" custom :id="`custom${index}`" @saved="updateCategory(index, $event)" />
            <TarButton class="ms-1" icon="fas fa-trash" :text="t('actions.remove')" variant="danger" @click="removeCategory(index)" />
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
