export interface GetAiModelResponse {
  id: string
  name: string
  version: string
  ollamaModelName: string
  isAvailable: boolean
}

export interface CreateTaskRequest {
  modelId: string
  priorityLevel?: string
}

export interface CreateTaskResponse {
  taskId: string
  modelId: string
  status: string
  ollamaModelName: string
}

export interface AddManualQuestionsRequest {
  questions: { questionNumber: number; text: string }[];
}

export interface StartTaskResponse {
  taskId: string
  status: string
}

export interface GetTaskStatusResponse {
  taskId: string
  objectName: string
  isReady: boolean
  status: string
}
