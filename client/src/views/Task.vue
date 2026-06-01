<script setup lang="ts">
import { storeToRefs } from 'pinia'

import AppHeader from '../components/layout/AppHeader.vue'
import AppSteps from '../components/common/AppSteps.vue'
import AppSourceUploader from '../components/common/AppSourceUploader.vue'
import AppButton from '../components/common/AppButton.vue'
import AppQuestionsUploader from '../components/common/AppQuestionsUploader.vue'
import AppUploadedFile from '../components/common/AppUploadedFile.vue'
import AppLoading from '../components/common/AppLoading.vue'

import { STEPS, useTaskStore } from '../stores/task.store.ts'

const taskStore = useTaskStore()

const { currentStep, sourceFiles, questionsText, questionsFile, loading } = storeToRefs(taskStore)

const setSourceFiles = (files: File[]) => {
  sourceFiles.value?.push(...files)
}

const removeSourceFile = (f: File) => {
  const filtered = sourceFiles.value?.filter(file => file !== f) || []
  sourceFiles.value = filtered
}

const setQuestionsFile = (file: File) => {
  questionsFile.value = file
}

const removeQuestionsFile = (_: File) => {
  questionsFile.value = null
}

const nextStep = () => {
  if (currentStep.value === 1) {
    if (sourceFiles.value?.length === 0) {
      alert('Загрузите хотя бы один источник')
      return
    }

    // TODO: Send files to server?

    currentStep.value++
  } else if (currentStep.value === 2) {
    if (!questionsText.value && !questionsFile.value) {
      alert('Введите вопросы в текстовое поле или загрузите файл с вопросами')
      return
    }

    // TODO: Send files to server?

    currentStep.value++
  } else if (currentStep.value === 3) {
    // TODO: Start generating result
    alert('Генерация отчета...')
  }
}
</script>

<template>
  <AppHeader />
  <section class="task section container">
    <AppSteps
      :currentStep="currentStep"
      :steps="STEPS"
    />
    <div v-if="currentStep === 1" class="step">
      <div class="step__source-upload">
        <AppSourceUploader @filesSelected="setSourceFiles" />
        <ul v-if="sourceFiles?.length" class="step__source-files">
          <li
            v-for="(file, index) in sourceFiles"
            class="step__source-files-item"
            :key="index"
          >
            <AppUploadedFile
              :file="file"
              @fileRemove="removeSourceFile"
            />
          </li>
        </ul>
      </div>
      <AppButton
        class="step__button"
        :text="'Продолжить'"
        :type="'button'"
        :disabled="!sourceFiles?.length"
        @click="nextStep"
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
    </div>
    <div v-else-if="currentStep === 2" class="step">
      <div class="step__questions">
        <textarea
          class="step__questions-textarea"
          id="questions-textarea"
          placeholder="Напишите здесь свои вопросы"
          v-model="questionsText"
        ></textarea>
        <AppQuestionsUploader
          class="step__questions-uploader"
          @fileSelected="setQuestionsFile"
        />
        <AppUploadedFile
          class="step__questions-file"
          v-if="questionsFile"
          :file="questionsFile"
          @fileRemove="removeQuestionsFile"
        />
      </div>
      <AppButton
        class="step__button"
        :text="'Получить ответы'"
        :type="'button'"
        :disabled="!questionsText && questionsFile === null"
        @click="nextStep"
      >
        <template #icon>
          <svg
            width="24" height="24" viewBox="0 0 24 24"
            fill="none"
          >
            <path
              d="M4.87881 18.6213C4.3162 18.0587 4.00013 17.2956 4.00013 16.5C3.99987 15.9522 4.14962 15.4148 4.43313 14.946C3.74867 14.8137 3.13161 14.4473 2.68788 13.9097C2.24416 13.372 2.00146 12.6966 2.00146 11.9995C2.00146 11.3024 2.24416 10.627 2.68788 10.0893C3.13161 9.55167 3.74867 9.18525 4.43313 9.053C4.15781 8.59796 4.00841 8.0779 4.00023 7.54611C3.99206 7.01432 4.12541 6.48992 4.38661 6.02663C4.64782 5.56334 5.02749 5.17781 5.48673 4.90956C5.94598 4.6413 6.46828 4.49996 7.00013 4.5C7.00013 3.83696 7.26353 3.20107 7.73237 2.73223C8.20121 2.26339 8.83709 2 9.50013 2C10.1632 2 10.7991 2.26339 11.2679 2.73223C11.7367 3.20107 12.0001 3.83696 12.0001 4.5C12.0001 3.83696 12.2635 3.20107 12.7324 2.73223C13.2012 2.26339 13.8371 2 14.5001 2C15.1632 2 15.7991 2.26339 16.2679 2.73223C16.7367 3.20107 17.0001 3.83696 17.0001 4.5C17.532 4.49996 18.0543 4.6413 18.5135 4.90956C18.9728 5.17781 19.3524 5.56334 19.6137 6.02663C19.8749 6.48992 20.0082 7.01432 20 7.54611C19.9919 8.0779 19.8425 8.59796 19.5671 9.053C20.2516 9.18525 20.8687 9.55167 21.3124 10.0893C21.7561 10.627 21.9988 11.3024 21.9988 11.9995C21.9988 12.6966 21.7561 13.372 21.3124 13.9097C20.8687 14.4473 20.2516 14.8137 19.5671 14.946C19.8427 15.4011 19.9923 15.9212 20.0006 16.4532C20.0089 16.9851 19.8756 17.5097 19.6144 17.9731C19.3532 18.4366 18.9734 18.8222 18.514 19.0905C18.0546 19.3588 17.5321 19.5002 17.0001 19.5C17.0001 20.163 16.7367 20.7989 16.2679 21.2678C15.7991 21.7366 15.1632 22 14.5001 22C13.8371 22 13.2012 21.7366 12.7324 21.2678C12.2635 20.7989 12.0001 20.163 12.0001 19.5C12.0001 20.163 11.7367 20.7989 11.2679 21.2678C10.7991 21.7366 10.1632 22 9.50013 22C8.83709 22 8.20121 21.7366 7.73237 21.2678C7.26353 20.7989 7.00013 20.163 7.00013 19.5C6.20448 19.5 5.44142 19.1839 4.87881 18.6213Z"
              stroke="#F8FAFC" stroke-width="1.5"
              stroke-linecap="round" stroke-linejoin="round"
            />
            <path
              d="M7.5 14.5L9.342 8.974C9.38824 8.83609 9.47664 8.7162 9.59471 8.63126C9.71278 8.54632 9.85455 8.50062 10 8.50062C10.1455 8.50062 10.2872 8.54632 10.4053 8.63126C10.5234 8.7162 10.6118 8.83609 10.658 8.974L12.5 14.5M15.5 8.5V14.5M8.5 12.5H11.5"
              stroke="#F8FAFC" stroke-width="1.5"
              stroke-linecap="round" stroke-linejoin="round"
            />
          </svg>
        </template>
      </AppButton>
    </div>
    <div v-else-if="currentStep === 3" class="step">
      <div class="step__result">
        <div class="step__result-description">
          <p>ИИ ищет ответы на ваши вопросы</p>
        </div>
        <div class="step__result-hint">
          <p>Пожалуйста, подождите</p>
        </div>
      </div>
      <AppLoading v-if="loading" class="step__loading" />
    </div>
  </section>
</template>

<style scoped lang="scss">
@use '../styles/helpers/' as *;

.task {
  display: flex;
  flex-direction: column;
  align-items: center;
  row-gap: rem(30);
}

.step {
  width: 100%;
  max-width: rem(840);

  &__source-upload {
    display: flex;
    flex-direction: column;
    gap: rem(16);

    &-input {
      padding: rem(10);
      border: 1px solid var(--color-gray);
      border-radius: rem(8);
      font-size: 1rem;
      width: 100%;
    }
  }

  &__source-files {
    display: flex;
    flex-direction: column;
    align-items: center;
    row-gap: rem(6);
  }

  &__questions {
    display: flex;
    flex-direction: column;
    row-gap: rem(24);
    
    &-textarea {
      padding: rem(24);
      width: 100%;
      height: rem(138);
      color: var(--color-dark);
      background-color: transparent;
      border: var(--border);
      border-radius: var(--border-radius);
      outline: none;
      resize: none;
      transition-duration: var(--transition-duration);

      @include hover {
        color: var(--color-dark);
        border-color: var(--color-dark);
      }

      &:focus {
        border-color: var(--color-dark);
      }

      &::placeholder {
        color: var(--color-gray);
      }
    }
  }
  
  &__result {
    @include fluid-text(20, 16);

    display: flex;
    flex-direction: column;
    align-items: center;
    row-gap: rem(6);

    &-description {
      font-weight: 600;
    }

    &-hint {
      color: var(--color-gray);
      font-weight: 500;
    }
  }

  &__button,
  &__loading {
    margin-inline: auto;

    &:not(:first-child) {
      margin-top: rem(60);
    }
  }
}
</style>
