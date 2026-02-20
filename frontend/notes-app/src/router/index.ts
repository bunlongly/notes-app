import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/notes'
    },
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/Login.vue')
    },
    {
      path: '/register',
      name: 'Register',
      component: () => import('@/views/Register.vue')
    },
    {
      path: '/notes',
      name: 'Notes',
      component: () => import('@/views/Notes.vue'),
      meta: { requiresAuth: true }
    }
  ]
});

// Navigation guard
router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore();

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login');
  } else if (
    (to.path === '/login' || to.path === '/register') &&
    authStore.isAuthenticated
  ) {
    next('/notes');
  } else {
    next();
  }
});

export default router;
