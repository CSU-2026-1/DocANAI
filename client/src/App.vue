<script setup lang="ts">
import { RouterView } from 'vue-router'
import { onMounted, ref } from 'vue'

// import { useErrorStore } from './stores/error.store'
import { useAuthStore } from './stores/auth.store'

// const errorStore = useErrorStore()
const authStore = useAuthStore()

const appReady = ref(false)

onMounted(async () => {
  if (authStore.accessToken && !authStore.userInfo) {
    await authStore.checkAuth()
  }

  appReady.value = true
})
</script>

<template>
  <!-- TODO: ErrorBanner -->
  
  <main>
    <div v-if="!appReady">Загрузка...</div>
    <RouterView v-else />
  </main>
</template>
