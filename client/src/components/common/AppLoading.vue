<template>
  <div class="loader"></div>
</template>

<style scoped lang="scss">
@use '../../styles/helpers/' as *;

.loader {
  position: relative;
  width: rem(90);
  height: rem(14);
  box-shadow: 0 rem(2) 0 var(--color-accent-1);
  clip-path: inset(-40px 0 -5px);

  &::before {
    content: '';
    position: absolute;
    inset: auto calc(50% - rem(17)) 0;
    height: rem(50);
    --g:no-repeat linear-gradient(var(--color-gray) 0 0);
    background: var(--g),var(--g),var(--g),var(--g);
    background-size: rem(16) rem(14);
    animation:
      l7-1 2s infinite linear,
      l7-2 2s infinite linear;
    }
}

@keyframes l7-1 {
  0%,
  100%  {background-position: 0 -50px,100% -50px}
  17.5% {background-position: 0 100%,100% -50px,0 -50px,100% -50px}
  35%   {background-position: 0 100%,100% 100% ,0 -50px,100% -50px}
  52.5% {background-position: 0 100%,100% 100% ,0 calc(100% - 16px),100% -50px}
  70%,
  98%  {background-position: 0 100%,100% 100% ,0 calc(100% - 16px),100% calc(100% - 16px)}
}

@keyframes l7-2 {
  0%,70% {transform:translate(0)}
  100%  {transform:translate(200%)}
}
</style>
