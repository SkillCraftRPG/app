<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AspectSelect from "@/components/aspects/AspectSelect.vue";
import type { AspectModel } from "@/types/aspects";
import type { Step3 } from "@/types/characters";
import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { t } = useI18n();

const aspect = ref<AspectModel>();
const aspects = ref<AspectModel[]>([]);

const isCompleted = computed<boolean>(() => requiredAspects.value === 0);
const requiredAspects = computed<number>(() => 2 - aspects.value.length);

defineEmits<{
  (e: "error", value: unknown): void;
}>();

function addAspect(): void {
  if (aspect.value) {
    aspects.value.push(aspect.value);
    aspect.value = undefined;
  }
}
function removeAspect(aspect: AspectModel): void {
  const index: number = aspects.value.findIndex(({ id }) => id === aspect.id);
  if (index >= 0) {
    aspects.value.splice(index, 1);
  }
}

type AttributeCategory = "mandatory" | "optional";
function formatAttributes(aspect: AspectModel, category: AttributeCategory): string {
  const attributes: string[] =
    category === "mandatory"
      ? [aspect.attributes.mandatory1, aspect.attributes.mandatory2]
          .filter((attribute) => Boolean(attribute))
          .map((attribute) => t(`game.attributes.${attribute}`))
      : [aspect.attributes.optional1, aspect.attributes.optional2]
          .filter((attribute) => Boolean(attribute))
          .map((attribute) => t(`game.attributes.${attribute}`));
  return attributes.join("<br />") || "—";
}
function formatSkills(aspect: AspectModel): string {
  const skills: string[] = [aspect.skills.discounted1, aspect.skills.discounted2].filter((skill) => Boolean(skill)).map((skill) => t(`game.skills.${skill}`));
  return skills.join("<br />") || "—";
} // TODO(fpion): refactor

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => {
  const payload: Step3 = { aspects: aspects.value };
  character.setStep3(payload);
});

onMounted(() => {
  const step3: Step3 | undefined = character.creation.step3;
  if (step3) {
    aspects.value = [...step3.aspects];
  }
});
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.aspects") }}</h3>
    <form @submit="onSubmit">
      <AspectSelect :disabled="requiredAspects === 0" :exclude="aspects" :model-value="aspect?.id" validation="server" @selected="aspect = $event">
        <template #append>
          <TarButton :disabled="!aspect" icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="addAspect" />
        </template>
      </AspectSelect>
      <p v-if="requiredAspects !== 0" class="text-danger">
        <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.aspects.select", { n: requiredAspects }) }}
      </p>
      <table v-if="aspects.length > 0" class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("name") }}</th>
            <th scope="col">{{ t("aspects.attributes.mandatory") }}</th>
            <th scope="col">{{ t("aspects.attributes.optional") }}</th>
            <th scope="col">{{ t("aspects.skills.label") }}</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="aspect in aspects" :key="aspect.id">
            <td>
              <RouterLink :to="{ name: 'AspectEdit', params: { id: aspect.id } }" target="_blank">
                <font-awesome-icon icon="fas fa-eye" /> {{ aspect.name }}
              </RouterLink>
            </td>
            <td v-html="formatAttributes(aspect, 'mandatory')"></td>
            <td v-html="formatAttributes(aspect, 'optional')"></td>
            <td v-html="formatSkills(aspect)"></td>
            <td>
              <TarButton icon="fas fa-trash" :text="t('actions.remove')" variant="danger" @click="removeAspect(aspect)" />
            </td>
          </tr>
        </tbody>
      </table>
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="character.goBack()" />
      <TarButton class="ms-1" :disabled="!isCompleted" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
