<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'

import AppButton from '../components/common/AppButton.vue'
import AppInput from '../components/common/AppInput.vue'

import { useAuthStore } from '../stores/auth.store'

import { ROUTES } from '../utils/constants.ts'

const router = useRouter()
const route = useRoute()

const authStore = useAuthStore()

const isLogin = ref(true)

const username = ref('')
const password = ref('')

const handleSubmit = async () => {
  if (!username.value || !password.value) return

  if (isLogin.value) {
    try {
      await authStore.login({
        username: username.value,
        password: password.value
      })

      const redirectPath = route.query.redirect as string

      router.push(redirectPath || `/${ROUTES.TASK}`)
    } catch (err) {
      console.error('Login failed:', err)
    }
  } else {
    try {
      await authStore.register({
        username: username.value,
        password: password.value,
        userType: 'Basic'
      })

      const redirectPath = route.query.redirect as string

      router.push(redirectPath || `/${ROUTES.TASK}`)
    } catch (err) {
      console.error('Register failed:', err)
    }
  }  
}
</script>

<template>
  <div class="auth">
    <h1 class="auth__logo h1">DocANAI</h1>
    <div class="auth__card">
      <header class="auth__card-header">
        <div class="auth__card-tabs">
          <div
            class="auth__card-tab"
            :class="{'auth__card-tab--active': isLogin}"
            @click="isLogin = true"
          >
            Вход
          </div>
          <div
            class="auth__card-tab"
            :class="{'auth__card-tab--active': !isLogin}"
            @click="isLogin = false"
          >
            Регистрация
          </div>
        </div>
      </header>
      <form
        class="auth__form"
        @submit.prevent="handleSubmit"
        autocomplete="off"
      >
        <div v-if="isLogin" class="auth__fields">
          <AppInput
            class="auth__field"
            v-model="username"
            :id="'login-username'"
            :placeholder="'Введите логин'"
            :type="'text'"
            :required="true"
            autocomplete="off"
          >
            <template #icon>
              <svg
                width="24" height="24" viewBox="0 0 24 24"
                fill="none"
              >
                <path
                  fill-rule="evenodd" clip-rule="evenodd"
                  d="M8 7C8 5.93913 8.42143 4.92172 9.17157 4.17157C9.92172 3.42143 10.9391 3 12 3C13.0609 3 14.0783 3.42143 14.8284 4.17157C15.5786 4.92172 16 5.93913 16 7C16 8.06087 15.5786 9.07828 14.8284 9.82843C14.0783 10.5786 13.0609 11 12 11C10.9391 11 9.92172 10.5786 9.17157 9.82843C8.42143 9.07828 8 8.06087 8 7ZM8 13C6.67392 13 5.40215 13.5268 4.46447 14.4645C3.52678 15.4021 3 16.6739 3 18C3 18.7956 3.31607 19.5587 3.87868 20.1213C4.44129 20.6839 5.20435 21 6 21H18C18.7956 21 19.5587 20.6839 20.1213 20.1213C20.6839 19.5587 21 18.7956 21 18C21 16.6739 20.4732 15.4021 19.5355 14.4645C18.5979 13.5268 17.3261 13 16 13H8Z"
                  fill="#B8B8B8"
                />
              </svg>
            </template>
          </AppInput>
          <AppInput
            class="auth__field"
            v-model="password"
            :id="'login-password'"
            :placeholder="'Введите пароль'"
            :type="'password'"
            :required="true"
            autocomplete="new-password"
          >
            <template #icon>
              <svg
                width="24" height="24" viewBox="0 0 24 24"
                fill="none"
              >
              <path
                fill-rule="evenodd" clip-rule="evenodd"
                d="M12 4C11.2044 4 10.4413 4.31607 9.87868 4.87868C9.31607 5.44129 9 6.20435 9 7V10H15V7C15 6.20435 14.6839 5.44129 14.1213 4.87868C13.5587 4.31607 12.7956 4 12 4ZM7 7V10H6C5.20435 10 4.44129 10.3161 3.87868 10.8787C3.31607 11.4413 3 12.2044 3 13V19C3 19.7956 3.31607 20.5587 3.87868 21.1213C4.44129 21.6839 5.20435 22 6 22H18C18.7956 22 19.5587 21.6839 20.1213 21.1213C20.6839 20.5587 21 19.7956 21 19V13C21 12.2044 20.6839 11.4413 20.1213 10.8787C19.5587 10.3161 18.7956 10 18 10H17V7C17 5.67392 16.4732 4.40215 15.5355 3.46447C14.5979 2.52678 13.3261 2 12 2C10.6739 2 9.40215 2.52678 8.46447 3.46447C7.52678 4.40215 7 5.67392 7 7ZM13 15C13 14.7348 12.8946 14.4804 12.7071 14.2929C12.5196 14.1054 12.2652 14 12 14C11.7348 14 11.4804 14.1054 11.2929 14.2929C11.1054 14.4804 11 14.7348 11 15V17C11 17.2652 11.1054 17.5196 11.2929 17.7071C11.4804 17.8946 11.7348 18 12 18C12.2652 18 12.5196 17.8946 12.7071 17.7071C12.8946 17.5196 13 17.2652 13 17V15Z"
                fill="#B8B8B8"
              />
              </svg>
            </template>
          </AppInput>
        </div>
        <div v-else class="auth__fields">
          <AppInput
            class="auth__field"
            v-model="username"
            :id="'register-username'"
            :placeholder="'Введите логин'"
            :type="'text'"
            :required="true"
            autocomplete="off"
          >
            <template #icon>
              <svg
                width="24" height="24" viewBox="0 0 24 24"
                fill="none"
              >
                <path
                  fill-rule="evenodd" clip-rule="evenodd"
                  d="M8 7C8 5.93913 8.42143 4.92172 9.17157 4.17157C9.92172 3.42143 10.9391 3 12 3C13.0609 3 14.0783 3.42143 14.8284 4.17157C15.5786 4.92172 16 5.93913 16 7C16 8.06087 15.5786 9.07828 14.8284 9.82843C14.0783 10.5786 13.0609 11 12 11C10.9391 11 9.92172 10.5786 9.17157 9.82843C8.42143 9.07828 8 8.06087 8 7ZM8 13C6.67392 13 5.40215 13.5268 4.46447 14.4645C3.52678 15.4021 3 16.6739 3 18C3 18.7956 3.31607 19.5587 3.87868 20.1213C4.44129 20.6839 5.20435 21 6 21H18C18.7956 21 19.5587 20.6839 20.1213 20.1213C20.6839 19.5587 21 18.7956 21 18C21 16.6739 20.4732 15.4021 19.5355 14.4645C18.5979 13.5268 17.3261 13 16 13H8Z"
                  fill="#B8B8B8"
                />
              </svg>
            </template>
          </AppInput>
          <AppInput
            class="auth__field"
            v-model="password"
            :id="'register-password'"
            :placeholder="'Придумайте пароль'"
            :type="'password'"
            :required="true"
            autocomplete="new-password"
          >
            <template #icon>
              <svg
                width="24" height="24" viewBox="0 0 24 24"
                fill="none"
              >
              <path
                fill-rule="evenodd" clip-rule="evenodd"
                d="M12 4C11.2044 4 10.4413 4.31607 9.87868 4.87868C9.31607 5.44129 9 6.20435 9 7V10H15V7C15 6.20435 14.6839 5.44129 14.1213 4.87868C13.5587 4.31607 12.7956 4 12 4ZM7 7V10H6C5.20435 10 4.44129 10.3161 3.87868 10.8787C3.31607 11.4413 3 12.2044 3 13V19C3 19.7956 3.31607 20.5587 3.87868 21.1213C4.44129 21.6839 5.20435 22 6 22H18C18.7956 22 19.5587 21.6839 20.1213 21.1213C20.6839 20.5587 21 19.7956 21 19V13C21 12.2044 20.6839 11.4413 20.1213 10.8787C19.5587 10.3161 18.7956 10 18 10H17V7C17 5.67392 16.4732 4.40215 15.5355 3.46447C14.5979 2.52678 13.3261 2 12 2C10.6739 2 9.40215 2.52678 8.46447 3.46447C7.52678 4.40215 7 5.67392 7 7ZM13 15C13 14.7348 12.8946 14.4804 12.7071 14.2929C12.5196 14.1054 12.2652 14 12 14C11.7348 14 11.4804 14.1054 11.2929 14.2929C11.1054 14.4804 11 14.7348 11 15V17C11 17.2652 11.1054 17.5196 11.2929 17.7071C11.4804 17.8946 11.7348 18 12 18C12.2652 18 12.5196 17.8946 12.7071 17.7071C12.8946 17.5196 13 17.2652 13 17V15Z"
                fill="#B8B8B8"
              />
              </svg>
            </template>
          </AppInput>
        </div>
        <AppButton
          v-if="isLogin"
          class="auth__card-button"
          :text="authStore.loading ? 'Вход...' : 'Войти в систему'"
          :type="'submit'"
          :disabled="authStore.loading"
        >
          <template #icon>
            <svg
              width="24" height="24" viewBox="0 0 24 24"
              fill="none"
            >
              <path d="M5.04 12.6H17.5104L12.3672 17.7432C12.132 17.9784 12.132 18.3576 12.3672 18.5928C12.4848 18.7104 12.6384 18.768 12.792 18.768C12.9456 18.768 13.0992 18.7104 13.2168 18.5928L19.3848 12.4248C19.62 12.1896 19.62 11.8104 19.3848 11.5752L13.2168 5.40717C12.9816 5.17197 12.6024 5.17197 12.3672 5.40717C12.132 5.64237 12.132 6.02157 12.3672 6.25677L17.5104 11.4H5.04C4.7088 11.4 4.44 11.6688 4.44 12C4.44 12.3312 4.7088 12.6 5.04 12.6Z" fill="white"/>
            </svg>
          </template>
        </AppButton>
        <AppButton
          v-else
          class="auth__card-button"
          :text="authStore.loading ? 'Создание аккаунта...' : 'Создать аккаунт'"
          :type="'submit'"
          :disabled="authStore.loading"
        >
          <template #icon>
            <svg
              width="24" height="24" viewBox="0 0 24 24"
              fill="none"
            >
              <path d="M5.04 12.6H17.5104L12.3672 17.7432C12.132 17.9784 12.132 18.3576 12.3672 18.5928C12.4848 18.7104 12.6384 18.768 12.792 18.768C12.9456 18.768 13.0992 18.7104 13.2168 18.5928L19.3848 12.4248C19.62 12.1896 19.62 11.8104 19.3848 11.5752L13.2168 5.40717C12.9816 5.17197 12.6024 5.17197 12.3672 5.40717C12.132 5.64237 12.132 6.02157 12.3672 6.25677L17.5104 11.4H5.04C4.7088 11.4 4.44 11.6688 4.44 12C4.44 12.3312 4.7088 12.6 5.04 12.6Z" fill="white"/>
            </svg>
          </template>
        </AppButton>
      </form>
    </div>
  </div>
</template>

<style scoped lang="scss">
@use '../styles/helpers/' as *;

.auth {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  row-gap: rem(32);
  height: 100%;

  &__card {
    display: flex;
    flex-direction: column;
    row-gap: rem(24);
    padding: rem(24);
    background-color: var(--color-light-alt);
    border: var(--border);
    border-radius: var(--border-radius);
    box-shadow: var(--shadow-dark);

    &-header {
      padding-bottom: rem(24);
      border-bottom: var(--border);
    }

    &-tabs {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      column-gap: rem(12);
    }

    &-tab {
      @include fluid-text(20, 16);

      padding: rem(12) rem(30);
      background-color: var(--color-light);
      border: var(--border);
      border-radius: var(--border-radius-small);
      color: var(--color-gray);
      text-align: center;
      font-weight: 600;
      transition-duration: var(--transition-duration);
      cursor: pointer;

      @include hover {
        border-color: var(--color-accent-1);
      }

      &--active {
        background-color: var(--color-accent-1);
        border-color: transparent;
        color: var(--color-light);
        box-shadow: var(--shadow-dark);

        @include hover {
          border-color: transparent;
        }
      }
    }

    &-button {
      width: 100%;
    }
  }

  &__form {
    display: flex;
    flex-direction: column;
    row-gap: rem(30);
  }

  &__fields {
    display: flex;
    flex-direction: column;
    row-gap: rem(10);
  }

  &__field {
    width: 100%;
  }
}
</style>
