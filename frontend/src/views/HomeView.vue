<script setup lang="ts">
import { ref } from "vue";

import type { UserType } from "@/types/account";
import WorldGateway from "@/components/worlds/WorldGateway.vue";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();

const mode = ref<UserType>(account.currentUser?.type ?? "Player");
</script>

<template>
  <main class="container">
    <div v-if="mode === 'Player'"></div>
    <WorldGateway v-else-if="mode === 'Gamemaster'" />
  </main>
</template>
