<script setup lang="ts">
import { useRouter } from 'vue-router'

import { useAuthStore } from '../stores/auth.store'

const router = useRouter()

const authStore = useAuthStore()

const handleLogout = () => {
  authStore.logout()

  router.push('/login')
}
</script>

<template>
  <div class="profile">
    <div class="profile__card">
      <h2 class="profile__card-title h2">Профиль пользователя</h2>
      <div v-if="authStore.userInfo" class="profile__card-info">
        <p>Имя пользователя: {{ authStore.userInfo.username }}</p>
        <p>Тип аккаунта: {{ authStore.userInfo.userType }}</p>
      </div>
    </div>
    <button class="logout-button" @click="handleLogout">Выйти</button>
  </div>
</template>
