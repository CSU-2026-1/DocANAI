import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth.store'

const Auth = () => import('../views/Auth.vue')
const Profile = () => import('../views/Profile.vue')

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: () => {
        const authStore = useAuthStore()
        return authStore.accessToken ? '/profile' : '/auth'
      }
    },
    {
      path: '/auth',
      name: 'auth',
      component: Auth,
      meta: { requiresGuest: true }
    },
    {
      path: '/profile',
      name: 'profile',
      component: Profile,
      meta: { requiresAuth: true }
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/',
    },
  ]
})

let authCheckPromise: Promise<boolean> | null = null

router.beforeEach(async (to, from) => {
  const authStore = useAuthStore()

  if (authStore.accessToken) {
    if (!authCheckPromise) {
      authCheckPromise = authStore.checkAuth()
    }

    try {
      await authCheckPromise
    } finally {
      authCheckPromise = null
    }
  }

  const isAuthenticated = authStore.userInfo

  if (to.meta.requiresAuth && !isAuthenticated) {
    return { path: '/auth', query: { redirect: to.fullPath } }
  }

  if (to.meta.requiresGuest && isAuthenticated) {
    const redirectPath = from.query.redirect as string
    const safePath = redirectPath && redirectPath.startsWith('/') ? redirectPath : '/profile'
    
    return safePath
  }

  return true
})

export default router
