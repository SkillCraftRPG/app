<script setup lang="ts">
import { onMounted } from "vue";
import { useRouter } from "vue-router";

import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const router = useRouter();

onMounted(() => {
  switch (account.currentUser?.type) {
    case "Gamemaster":
      router.push({ name: "WorldList" });
      break;
    default:
      break;
  }
});
</script>

<template>
  <main class="container"></main>
</template>
