import { createRouter, createWebHistory } from "vue-router";

import HomeView from "./views/HomeView.vue";
import { useAccountStore } from "./stores/account";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "Home",
      component: HomeView,
      meta: { isPublic: true },
    },
    // Account
    {
      name: "Profile",
      path: "/profile",
      component: () => import("./views/account/ProfileView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignIn",
      path: "/auth",
      component: () => import("./views/account/SignInView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/logout",
      component: () => import("./views/account/SignOutView.vue"),
      meta: { isPublic: true },
    },
    // Customizations
    {
      name: "Customizations",
      path: "/customizations",
      component: () => import("./views/customizations/CustomizationsView.vue"),
    },
    {
      name: "Customization",
      path: "/customizations/:id",
      component: () => import("./views/customizations/CustomizationView.vue"),
    },
    // Languages
    {
      name: "Languages",
      path: "/languages",
      component: () => import("./views/languages/LanguagesView.vue"),
    },
    {
      name: "Language",
      path: "/languages/:id",
      component: () => import("./views/languages/LanguageView.vue"),
    },
    // Scripts
    {
      name: "Scripts",
      path: "/scripts",
      component: () => import("./views/scripts/ScriptsView.vue"),
    },
    {
      name: "Script",
      path: "/scripts/:id",
      component: () => import("./views/scripts/ScriptView.vue"),
    },
    // Sheets
    {
      name: "CharacterSheets",
      path: "/sheets",
      component: () => import("./views/sheets/CharacterSheets.vue"),
    },
    // Worlds
    {
      name: "Worlds",
      path: "/worlds",
      component: () => import("./views/worlds/WorldsView.vue"),
    },
    {
      name: "World",
      path: "/worlds/:id",
      component: () => import("./views/worlds/WorldView.vue"),
    },
    // NotFound
    {
      name: "NotFound",
      path: "/:pathMatch(.*)*",
      component: () => import("./views/NotFound.vue"),
      // route level code-splitting
      // this generates a separate chunk (NotFound.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      meta: { isPublic: true },
    },
  ],
});

router.beforeEach(async (to) => {
  const account = useAccountStore();
  if (!to.meta.isPublic && !account.currentUser) {
    return { name: "SignIn", query: { redirect: to.fullPath } };
  }
});

export default router;
