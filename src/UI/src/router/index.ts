import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import Login from "../components/Login.vue";
import Register from "../components/Register.vue";
import Dashboard from "../components/Dashboard.vue";
import { isAuthenticated } from "../utils/auth";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    redirect: "/login",
  },
  {
    path: "/login",
    component: Login,
    meta: {
      requiresAuth: false,
      redirectIfAuth: true,
    },
  },
  {
    path: "/register",
    component: Register,
    meta: {
      requiresAuth: false,
      redirectIfAuth: true,
    },
  },
  {
    path: "/dashboard",
    component: Dashboard,
    meta: {
      requiresAuth: true,
    },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Navigation guard
router.beforeEach(async (to, from, next) => {
  const isUserAuthenticated = await isAuthenticated();

  // If route requires auth and user is not authenticated
  if (to.meta.requiresAuth && !isUserAuthenticated) {
    next("/login");
    return;
  }

  // If user is authenticated and tries to access login/register
  if (to.meta.redirectIfAuth && isUserAuthenticated) {
    next("/dashboard");
    return;
  }

  next();
});

export default router;
