import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useErrorStore = defineStore('error', () => {
  const hasError = ref(false)
  const errorTitle = ref('')
  const errorMessage = ref('')
  const errorCode = ref<number | null>(null)
  const fatal = ref(false)

  const setError = (title: string, message: string, code?: number, isFatal = false) => {
    hasError.value = true
    errorTitle.value = title
    errorMessage.value = message
    errorCode.value = code || null
    fatal.value = isFatal

    if (!isFatal) {
      setTimeout(() => {
        clearError()
      }, 5000)
    }
  }

  const clearError = () => {
    hasError.value = false
    errorTitle.value = ''
    errorMessage.value = ''
    errorCode.value = null
    fatal.value = false
  }

  return {
    hasError,
    errorTitle,
    errorMessage,
    errorCode,
    fatal,
    setError,
    clearError
  }
})
