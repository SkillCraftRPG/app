<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import type { UserType } from "@/types/account";

const { t } = useI18n();

defineProps<{
  isLoading?: boolean;
}>();

const userType = ref<UserType>("Player");

const emit = defineEmits<{
  (e: "back"): void;
  (e: "continue", value: UserType): void;
}>();

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => {
  emit("continue", userType.value);
});
</script>

<template>
  <div>
    <h3>{{ t("users.profile.usage.title") }}</h3>
    <p>
      <i><font-awesome-icon icon="fas fa-palette" /> {{ t("users.profile.usage.lead") }}</i>
    </p>
    <form @submit="onSubmit">
      <div class="mb-3">
        <input class="btn-check" id="player" name="player" type="radio" value="player" :checked="userType === 'Player'" @change="userType = 'Player'" />
        <label class="btn btn-outline-primary me-1" for="player">
          <font-awesome-icon icon="fas fa-dice" /> {{ t("users.profile.usage.type.Player.label") }}
        </label>
        <input
          class="btn-check"
          id="gamemaster"
          name="gamemaster"
          type="radio"
          value="gamemaster"
          :checked="userType === 'Gamemaster'"
          @change="userType = 'Gamemaster'"
        />
        <label class="btn btn-outline-primary mx-1" for="gamemaster">
          <font-awesome-icon icon="fas fa-spaghetti-monster-flying" /> {{ t("users.profile.usage.type.Gamemaster.label") }}
        </label>
        <span class="ms-1"> <font-awesome-icon icon="fas fa-circle-info" /> {{ t(`users.profile.usage.type.${userType}.help`) }} </span>
      </div>
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="$emit('back')" />
      <TarButton
        class="ms-1"
        :disabled="isLoading"
        icon="fas fa-check"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.complete')"
        variant="success"
        type="submit"
      />
    </form>
  </div>
</template>
