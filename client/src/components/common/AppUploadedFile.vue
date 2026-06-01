<script setup lang="ts">
import { getFileType, VALID_FILE_TYPES } from '../../utils/files'

const props = defineProps<{
  file: File
}>()

const emit = defineEmits<{
  (e: 'fileRemove', file: File): void
}>()
</script>

<template>
  <div class="file">
    <img
      v-if="getFileType(file) === VALID_FILE_TYPES.TXT"
      class="file__image"
      src="../../assets/images/txt.png"
      alt=""
      width="24" height="24"
    >
    <img
      v-else-if="getFileType(file) === VALID_FILE_TYPES.DOCX"
      class="file__image"
      src="../../assets/images/docx.png"
      alt=""
      width="24" height="24"
    >
    <img
      v-if="getFileType(file) === VALID_FILE_TYPES.PDF"
      class="file__image"
      src="../../assets/images/pdf.png"
      alt=""
      width="24" height="24"
    >
    <div class="file__name">{{ file.name }}</div>
    <button
      class="file__remove"
      type="button"
      @click="emit('fileRemove', file)"
    >
      ✕
    </button>
  </div>
</template>

<style scoped lang="scss">
@use '../../styles/helpers/' as *;

.file {
  @include fluid-text(16, 14);

  display: flex;
  align-items: center;
  column-gap: rem(12);

  &__image {
    @include square(24);
  }

  &__remove {
    @include square(24);
    @include flex-center;

    color: var(--color-light);
    background-color: var(--color-red);
    border: rem(1) solid var(--color-red);
    border-radius: 50%;
    transition-duration: var(--transition-duration);

    @include hover {
      background-color: transparent;
      color: var(--color-red);
    }
  }
}
</style>
