<template>
  <div class="d-flex flex-column min-vh-100">
    <LayoutNavbar />
    <div class="flex-grow-1 d-flex flex-column py-3">
      <RouterView />
      <Transition name="fade">
        <button
          v-if="showScrollTop"
          type="button"
          class="btn btn-lg btn-primary btn-scroll position-fixed end-0 m-3 rounded-circle shadow"
          :style="{ bottom: `${footerOverlap}px` }"
          @click="scrollToTop"
        >
          <font-awesome-icon icon="fas fa-arrow-up" />
        </button>
      </Transition>
    </div>
    <LayoutFooter id="footer" />
    <TarToaster :toasts="toasts.toasts" @hidden="toasts.remove" />
  </div>
</template>

<script setup lang="ts">
import { RouterView, useRoute, useRouter } from "vue-router";
import { onBeforeUnmount, onMounted, provide, ref } from "vue";

import LayoutFooter from "./components/layout/LayoutFooter.vue";
import LayoutNavbar from "./components/layout/LayoutNavbar.vue";
import TarToaster from "@/components/tar/TarToaster.vue";
import { StatusCodes, type ApiFailure } from "./types/api";
import { handleErrorKey } from "./inject";
import { useAccountStore } from "./stores/account";
import { useToastStore } from "./stores/toast";

const account = useAccountStore();
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const footerOverlap = ref<number>(0);
const showScrollTop = ref<boolean>(false);

function handleError(e: unknown): void {
  if (e) {
    const { status } = e as ApiFailure;
    if (status === StatusCodes.Unauthorized) {
      account.signOut("expired");
      router.push({ name: "SignIn", query: { redirect: route.fullPath } });
    } else {
      console.error(e);
      toasts.error();
    }
  } else {
    toasts.error();
  }
}
provide(handleErrorKey, handleError);

function scrollToTop(): void {
  window.history.replaceState(window.history.state, "", window.location.pathname + window.location.search);
  window.scrollTo({ top: 0, left: 0, behavior: "smooth" });
}

function update(): void {
  showScrollTop.value = window.scrollY > 200;

  const footer: HTMLElement | null = document.getElementById("footer");
  if (footer) {
    const footerTop: number = footer.getBoundingClientRect().top + window.scrollY;
    const viewportBottom: number = window.scrollY + window.innerHeight;
    const overlap: number = viewportBottom - footerTop;
    footerOverlap.value = Math.max(0, Math.min(footer.offsetHeight, overlap));
  } else {
    footerOverlap.value = 0;
  }
}

onMounted(() => {
  update();
  window.addEventListener("resize", update);
  window.addEventListener("scroll", update, { passive: true });
});

onBeforeUnmount(() => {
  window.removeEventListener("resize", update);
  window.removeEventListener("scroll", update);
});
</script>
