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
    // Castes
    {
      name: "Castes",
      path: "/castes",
      component: () => import("./views/castes/CastesView.vue"),
    },
    {
      name: "Caste",
      path: "/castes/:id",
      component: () => import("./views/castes/CasteView.vue"),
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
    // Educations
    {
      name: "Educations",
      path: "/educations",
      component: () => import("./views/educations/EducationsView.vue"),
    },
    {
      name: "Education",
      path: "/educations/:id",
      component: () => import("./views/educations/EducationView.vue"),
    },
    // Items
    {
      name: "Items",
      path: "/items",
      component: () => import("./views/items/ItemsView.vue"),
    },
    {
      name: "Item",
      path: "/items/:id",
      component: () => import("./views/items/ItemView.vue"),
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
    // Lineages
    {
      name: "Lineages",
      path: "/lineages",
      component: () => import("./views/lineages/LineagesView.vue"),
    },
    {
      name: "Lineage",
      path: "/lineages/:id",
      component: () => import("./views/lineages/LineageView.vue"),
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
    // Spells
    {
      name: "Spells",
      path: "/spells",
      component: () => import("./views/spells/SpellsView.vue"),
    },
    {
      name: "Spell",
      path: "/spells/:id",
      component: () => import("./views/spells/SpellView.vue"),
    },
    // Talents
    {
      name: "Talents",
      path: "/talents",
      component: () => import("./views/talents/TalentsView.vue"),
    },
    {
      name: "Talent",
      path: "/talents/:id",
      component: () => import("./views/talents/TalentView.vue"),
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
