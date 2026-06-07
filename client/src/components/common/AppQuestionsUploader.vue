<script setup lang="ts">
import { ref } from 'vue'

import { isFileValid } from '../../utils/files'

const emit = defineEmits<{
  (e: 'fileSelected', file: File): void
}>()

const fileInputRef = ref<HTMLInputElement | null>(null)
const file = ref<File | null>(null)

const triggerFileInput = () => {
  fileInputRef.value?.click()
}

const handleFileChange = (event: Event) => {
  const input = event.target as HTMLInputElement
  const inputFile = input.files?.item(0)

  if (inputFile) file.value = inputFile
  
  if (file.value) processFile(file.value)
}

const processFile = (f: File) => {
  // TODO: File validation
  if (!isFileValid(f)) {
    file.value = null
    return
  }

  emit('fileSelected', f)

  file.value = null
}
</script>

<template>
  <div
    class="uploader"
    @click="triggerFileInput"
  >
    <input
      class="visually-hidden"
      ref="fileInputRef"
      type="file"
      accept=".docx,.pdf"
      @change="handleFileChange"
    >
    <div class="uploader__content">
      <svg
        class="uploader__icon"
        width="24" height="24" viewBox="0 0 24 24"
        fill="none"
      >
        <path d="M20.28 2.52C16.8 -0.84 11.28 -0.84 7.79995 2.52L1.19995 9.24L2.87995 10.92L9.47995 4.32C12 1.8 16.08 1.8 18.48 4.32C21 6.84 21 10.92 18.48 13.32L11.28 20.52C9.83995 21.96 7.31995 21.96 5.75995 20.52C4.19995 18.96 4.19995 16.56 5.75995 15L12 8.76C12.48 8.28 13.32 8.28 13.92 8.76C14.4 9.24 14.4 10.08 13.92 10.68L8.39995 16.32L10.08 18L15.72 12.36C17.16 10.92 17.16 8.52 15.72 7.08C14.28 5.64 11.88 5.64 10.44 7.08L4.19995 13.32C1.79995 15.72 1.79995 19.8 4.19995 22.2C5.39995 23.4 6.95995 24 8.63995 24C10.32 24 11.88 23.4 13.08 22.2L20.28 15C23.64 11.64 23.64 6 20.28 2.52Z" fill="#C1C8D0"/>
      </svg>
      <div class="uploader__text">
        <p>Прикрепить файл с вопросами</p>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
@use '../../styles/helpers' as *;

.uploader {
  color: var(--color-gray);
  cursor: pointer;
  transition-duration: var(--transition-duration);
  
  @include hover {
    color: var(--color-dark);
  }

  &__content {
    display: flex;
    align-items: center;
    column-gap: rem(16);
  }
}
</style>
