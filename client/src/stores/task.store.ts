import { defineStore } from 'pinia'
import { ref } from 'vue'

export const STEPS = [
  { title: 'Добавьте источники' },
  { title: 'Ваши вопросы' },
  { title: 'Результат' }
]

export const useTaskStore = defineStore('task', () => {
  const currentStep = ref(1)
  const sourceFiles = ref<File[]>([])
  const questionsText = ref('')
  const questionsFile = ref<File | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const resetTask = () => {
    currentStep.value = 1
    sourceFiles.value = []
    questionsText.value = ''
    questionsFile.value = null
  }

  return {
    currentStep,
    sourceFiles,
    questionsText,
    questionsFile,
    loading,
    error,
    resetTask
  }
})
