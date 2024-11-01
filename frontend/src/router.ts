import { createRouter, createWebHistory } from "vue-router";

import HomeView from "./views/HomeView.vue";

import { useAccountStore } from "./stores/account";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      name: "Home",
      path: "/",
      component: HomeView,
    },
    // Account
    {
      name: "Profile",
      path: "/profile",
      component: () => import("./views/account/ProfileView.vue"),
    },
    {
      name: "ProfileCompletion",
      path: "/profile/complete/:token",
      component: () => import("./views/account/ProfileCompletion.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignIn",
      path: "/sign-in",
      component: () => import("./views/account/SignInView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/sign-out",
      component: () => import("./views/account/SignOutView.vue"),
    },
    // Aspects
    {
      name: "AspectEdit",
      path: "/worlds/:slug/aspects/:id",
      component: () => import("./views/aspects/AspectEdit.vue"),
    },
    {
      name: "AspectList",
      path: "/worlds/:slug/aspects",
      component: () => import("./views/aspects/AspectList.vue"),
    },
    // Aspects
    {
      name: "CasteEdit",
      path: "/worlds/:slug/castes/:id",
      component: () => import("./views/castes/CasteEdit.vue"),
    },
    {
      name: "CasteList",
      path: "/worlds/:slug/castes",
      component: () => import("./views/castes/CasteList.vue"),
    },
    // Worlds
    {
      name: "WorldIndex",
      path: "/worlds/:slug",
      component: () => import("./views/worlds/WorldIndex.vue"),
    },
    {
      name: "WorldList",
      path: "/worlds",
      component: () => import("./views/worlds/WorldList.vue"),
    },
    // NotFound
    {
      name: "NotFound",
      path: "/:pathMatch(.*)*",
      // route level code-splitting
      // this generates a separate chunk (ProfileView.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("./views/NotFound.vue"),
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
