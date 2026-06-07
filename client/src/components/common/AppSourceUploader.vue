<script setup lang="ts">
import { ref } from 'vue'

import { isFileValid } from '../../utils/files'

const emit = defineEmits<{
  (e: 'filesSelected', files: File[]): void
}>()

const fileInputRef = ref<HTMLInputElement | null>(null)
const isDragover = ref(false)

const files = ref<File[] | null>([])

const triggerFileInput = () => {
  fileInputRef.value?.click()
}

const handleFilesChange = (event: Event) => {
  const input = event.target as HTMLInputElement

  if (input.files && input.files.length)
    files.value?.push(...Array.from(input.files))

  if (files.value) processFiles(files.value)

  input.value = ''
}

const handleDrop = (event: DragEvent) => {
  isDragover.value = false

  const selectedFiles = event.dataTransfer?.files
  if (selectedFiles && selectedFiles.length)
    files.value?.push(...Array.from(selectedFiles))

  if (files.value) processFiles(files.value)
}

const processFiles = (array: File[]) => {
  // TODO: Files validation
  if (!array.every(file => isFileValid(file))) {
    files.value = []
    return
  }

  emit('filesSelected', array)

  files.value = []
}
</script>

<template>
  <div
    class="uploader"
    :class="{ 'uploader--dragover': isDragover }"
    @dragover.prevent="isDragover = true"
    @dragleave.prevent="isDragover = false"
    @drop.prevent="handleDrop"
    @click="triggerFileInput"
  >
    <input
      class="visually-hidden"
      ref="fileInputRef"
      type="file"
      accept=".docx,.pdf"
      multiple
      @change="handleFilesChange"
    >
    <div class="uploader__content">
      <svg
        class="uploader__icon"
        width="48" height="48" viewBox="0 0 48 48"
        fill="none"
      >
        <path d="M31.065 21.435C31.2056 21.5745 31.3172 21.7404 31.3933 21.9231C31.4695 22.1059 31.5087 22.302 31.5087 22.5C31.5087 22.698 31.4695 22.8941 31.3933 23.0769C31.3172 23.2597 31.2056 23.4256 31.065 23.565C30.9256 23.7056 30.7597 23.8172 30.5769 23.8933C30.3941 23.9695 30.198 24.0087 30 24.0087C29.802 24.0087 29.6059 23.9695 29.4231 23.8933C29.2403 23.8172 29.0744 23.7056 28.935 23.565L25.5 20.115V31.5C25.5 31.8978 25.342 32.2794 25.0607 32.5607C24.7794 32.842 24.3978 33 24 33C23.6022 33 23.2206 32.842 22.9393 32.5607C22.658 32.2794 22.5 31.8978 22.5 31.5V20.115L19.065 23.565C18.7825 23.8475 18.3995 24.0061 18 24.0061C17.6005 24.0061 17.2175 23.8475 16.935 23.565C16.6525 23.2826 16.4939 22.8995 16.4939 22.5C16.4939 22.1006 16.6525 21.7175 16.935 21.435L22.935 15.435C23.0744 15.2944 23.2403 15.1828 23.4231 15.1067C23.6059 15.0305 23.802 14.9913 24 14.9913C24.198 14.9913 24.3941 15.0305 24.5769 15.1067C24.7597 15.1828 24.9256 15.2944 25.065 15.435L31.065 21.435ZM40.5 14.58V40.5C40.5 41.6935 40.0259 42.8381 39.182 43.682C38.3381 44.5259 37.1935 45 36 45H12C10.8065 45 9.66193 44.5259 8.81802 43.682C7.97411 42.8381 7.5 41.6935 7.5 40.5V7.50001C7.5 6.30653 7.97411 5.16194 8.81802 4.31803C9.66193 3.47411 10.8065 3.00001 12 3.00001H30.09C30.7511 2.99893 31.4043 3.14353 32.0032 3.42353C32.6021 3.70352 33.1319 4.11203 33.555 4.62001L39.45 11.7C40.1263 12.5075 40.4978 13.5267 40.5 14.58ZM31.5 10.5C31.5 10.8978 31.658 11.2794 31.9393 11.5607C32.2206 11.842 32.6022 12 33 12H35.79L31.5 6.84001V10.5ZM37.5 15H33C31.8065 15 30.6619 14.5259 29.818 13.682C28.9741 12.8381 28.5 11.6935 28.5 10.5V6.00001H12C11.6022 6.00001 11.2206 6.15804 10.9393 6.43935C10.658 6.72065 10.5 7.10218 10.5 7.50001V40.5C10.5 40.8978 10.658 41.2794 10.9393 41.5607C11.2206 41.842 11.6022 42 12 42H36C36.3978 42 36.7794 41.842 37.0607 41.5607C37.342 41.2794 37.5 40.8978 37.5 40.5V15Z" fill="#C1C8D0"/>
      </svg>
      <div class="uploader__text">
        <p>
          Выберите файлы или перетащите их сюда<br/>
          DOCX, PDF
        </p>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
@use '../../styles/helpers/' as *;

.uploader {
  @include flex-center;

  padding: rem(24) rem(60);
  color: var(--color-gray);
  border: var(--border);
  border-radius: var(--border-radius);
  cursor: pointer;
  transition-duration: var(--transition-duration);

  @include hover {
    color: var(--color-dark);
    border-color: var(--color-accent-2);
  }

  &--dragover {
    color: var(--color-dark);
    border-color: var(--color-accent-2);
  }

  &__content {
    @include flex-center;

    flex-direction: column;
    text-align: center;
    row-gap: rem(24);
  }
}
</style>
