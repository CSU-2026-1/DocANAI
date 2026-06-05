import { defineStore } from 'pinia'
import { ref } from 'vue'

import api from '../services/api'

import type { CreateTaskRequest, CreateTaskResponse, AddManualQuestionsRequest, StartTaskResponse, GetTaskStatusResponse, GetAiModelResponse } from '../types/api'

export const STEPS = [
  { title: 'Добавьте источники' },
  { title: 'Ваши вопросы' },
  { title: 'Результат' }
]

export const useTaskStore = defineStore('task', () => {
  const selectedModelId = ref<string | null>(null)

  const currentStep = ref(1)
  const sourceFiles = ref<File[]>([])
  const questionsText = ref('')
  const questionsFile = ref<File | null>(null)
  
  const taskId = ref<string | null>(null)
  const reportObjectName = ref<string | null>(null)

  const loading = ref(false)
  const error = ref<string | null>(null)
  let pollInterval: number | null = null

  const resetTask = () => {
    if (pollInterval) clearInterval(pollInterval)
    currentStep.value = 1
    sourceFiles.value = []
    questionsText.value = ''
    questionsFile.value = null
    taskId.value = null
    reportObjectName.value = null
    loading.value = false
    error.value = null
  }

  const loadDefaultModelId = async () => {
    loading.value = true
    error.value = null

    try {
      const response = await api.get<GetAiModelResponse[]>('/ai-models');
      let models: GetAiModelResponse[] = [];

      if (response.status === 204 || !Array.isArray(response.data)) {
        models = []
      } else {
        models = response.data
      }

      const availableModel = models.find(m => m.isAvailable)
      if (availableModel) {
        selectedModelId.value = availableModel.id
      } else {
        throw new Error('No available AI model')
      }
    } catch (err: any) {
      error.value = 'Ошибка загрузки модели AI'
      throw err
    } finally {
      loading.value = false
    }
  }

  const goToNextStep = () => {
    if (currentStep.value < STEPS.length) currentStep.value++
  }

  const goToPreviousStep = () => {
    if (currentStep.value > 1) currentStep.value--
  }

  const submitTask = async (priorityLevel = 'normal') => {
    if (!selectedModelId.value) {
      await loadDefaultModelId()
    }

    if (!sourceFiles.value.length) throw new Error('Нет файлов-источников')
    if (!questionsText.value && !questionsFile.value) throw new Error('Нет вопросов')

    loading.value = true
    error.value = null

    try {
      const { data: taskData } = await api.post<CreateTaskResponse>('/tasks', {
        modelId: selectedModelId.value,
        priorityLevel
      } as CreateTaskRequest)

      taskId.value = taskData.taskId

      const sourceUploads = sourceFiles.value.map(file => {
        const formData = new FormData()
        formData.append('file', file)
        
        return api.post(`/filestorage/upload?taskId=${taskId.value}&isQuestionFile=false`, formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
      })

      await Promise.all(sourceUploads)

      if (questionsText.value.trim()) {
        const lines = questionsText.value.split(/\r?\n/).filter(l => l.trim())
        const questions = lines.map((text, idx) => ({ questionNumber: idx + 1, text }))

        await api.post<AddManualQuestionsRequest>(`/tasks/${taskId.value}/questions/manual`, { questions })
      } else if (questionsFile.value) {
        const formData = new FormData()
        formData.append('file', questionsFile.value)

        await api.post(`/filestorage/upload?taskId=${taskId.value}&isQuestionFile=true`, formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        });
      }

      await api.post<StartTaskResponse>(`/tasks/${taskId.value}/start`)

      currentStep.value = 3

      await pollTaskStatus(10, 60)
    } catch (err: any) {
      error.value = err.response?.data?.error || 'Ошибка при отправке задачи'
      throw err
    } finally {
      loading.value = false
    }
  }

  const pollTaskStatus = (intervalSeconds = 10, maxAttempts = 60) => {
    if (!taskId.value) throw new Error('Task not created')
    
    return new Promise((resolve, reject) => {
      let attempts = 0
      
      pollInterval = window.setInterval(async () => {
        attempts++

        try {
          const { data } = await api.get<GetTaskStatusResponse>(`/tasks/${taskId.value}/status`)

          if (data.isReady && data.objectName) {
            reportObjectName.value = data.objectName

            clearInterval(pollInterval!)
            resolve(data.objectName)
          } else if (attempts >= maxAttempts) {
            clearInterval(pollInterval!)
            reject(new Error('Timeout waiting for task status'))
          }
        } catch (err) {
          if (attempts >= maxAttempts) {
            clearInterval(pollInterval!)
            reject(err)
          }
        }
      }, intervalSeconds * 1000)
    })
  }

  const downloadReport = async () => {
    if (!reportObjectName.value) throw new Error('Report not ready')

    try {
      const response = await api.get('/filestorage/download', {
        params: { objectName: reportObjectName.value },
        responseType: 'blob'
      })

      const url = window.URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')

      link.href = url
      link.setAttribute('download', reportObjectName.value.split('/').pop() || 'result.xlsx')

      document.body.appendChild(link)

      link.click()
      link.remove()

      window.URL.revokeObjectURL(url)
    } catch (err: any) {
      error.value = 'Ошибка скачивания отчёта'
      throw err
    }
  };

  return {
    currentStep,
    sourceFiles,
    questionsText,
    questionsFile,
    taskId,
    reportObjectName,
    loading,
    error,
    loadDefaultModelId,
    resetTask,
    goToNextStep,
    goToPreviousStep,
    submitTask,
    downloadReport
  }
})
