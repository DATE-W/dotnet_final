<template>
    <!-- 默认指示器显示在内部 -->
    <el-carousel style="height: 100vh" arrow="never">
      <el-carousel-item v-for="item in 4" :key="item" style="height: 100vh">
        <img :src="getImageUrl(item)" alt="Carousel Image" class="carousel-image" />
      </el-carousel-item>
    </el-carousel>
  </template>
  
  <style scoped>
  .carousel-image {
    width: 100%;
    height: 100%;
    object-fit: scale-down;
  }
  </style>
    
  <script>
  import carousel1 from '../assets/img/carousel1.png';
  import carousel2 from '../assets/img/carousel2.png';
  import carousel3 from '../assets/img/carousel3.png';
  import carousel4 from '../assets/img/carousel4.png';
  
  export default {
    methods: {
      getImageUrl(item) {
        switch (item) {
          case 1:
            return carousel1;
          case 2:
            return carousel2;
          case 3:
            return carousel3;
          case 4:
            return carousel4;
          default:
            return '';
        }
      }
    }
  }
  
  </script>
  
  
  <style scoped></style>
    