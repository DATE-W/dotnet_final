<template>
  <div>
    <!-- 顶部导航栏 -->
    <my-nav></my-nav>
    <!-- 标题+tag筛选 -->
    <el-row>
      <p class="titleLeagueLeft">足坛视频</p>
      <el-icon class="Gossipicon">
        <Promotion />
      </el-icon>
      <div class="GossipButton">
        <el-row class="mb-4">
          <el-button plain @click="selectVideo('中超')">中超</el-button>
          <el-button type="primary" plain @click="selectVideo('英超')">英超</el-button>
          <el-button type="success" plain @click="selectVideo('西甲')">西甲</el-button>
          <el-button type="info" plain @click="selectVideo('意甲')">意甲</el-button>
          <el-button type="warning" plain @click="selectVideo('德甲')">德甲</el-button>
          <el-button type="danger" plain @click="selectVideo('法甲')">法甲</el-button>
          <el-button plain @click="selectVideo('')">ALL</el-button>
        </el-row>
      </div>
    </el-row>
    <div class="line" style="width: 40vw;height: 0.2px;top:-1vh;left:5%;"></div>

    <el-row :gutter="20">
      <el-col :span="8">
        <!-- 上侧走马灯 -->
        <div class="VideoCarousel">
          <!-- 走马灯 -->
          <div>
            <el-carousel :interval="3000" class="Carousel" style="height: 20vw;width: auto;">
              <el-carousel-item v-for="(item, index) in VideoCarousel" :key="index">
                <img referrerPolicy='no-referrer' :src="item.cover" alt="carousel image" class="imgANO"
                  @click="openLink(item.urllink)">
                <div class="description" v-if="item.title != null" @click="openLink(item.urllink)">{{
                  truncateText(item.title, 30) }}</div>
              </el-carousel-item>
            </el-carousel>
          </div>
        </div>
      </el-col>
      <el-col :span="16">
        <!-- 上侧右侧推荐视频 -->
        <div class="VideoRecommend">
          <el-row :gutter="10" v-for="(row, index) in 2" :key="index">
            <el-col :span="6" v-for="(item, index) in VideoRight.slice((row - 1) * 3, row * 3)" :key="index"
              style="margin-top: 8px;">
              <img referrerPolicy='no-referrer' :src="item.cover" class="imgRightItem" @click="openLink(item.urllink)"
                alt="image" />
              <a class="Rightdescription" v-if="item.title != null" @click="openLink(item.urlink)">{{
                truncateText(item.title, 14) }}</a>
            </el-col>
          </el-row>
        </div>
      </el-col>
      <div class="shiftButton" @click="updata()">
        <el-icon class="Shifticon">
          <Switch />
        </el-icon>
        <a class="shift">换一批</a>
      </div>
    </el-row>

    <el-row :gutter="20">
      <!-- 下部全部视频 -->
      <div class="Videocontainer">
        <el-row :gutter="10" v-for="(row, index) in 5" :key="index">
          <el-col :span="4" v-for="(item, index) in VideoAll.slice((row - 1) * 5, row * 5)" :key="index"
            style="margin-top: 50px;">
            <img referrerPolicy='no-referrer' :src="item.cover" class="imgItem" @click="openLink(item.urllink)"
              alt="image" />
            <a class="Alldescription" v-if="item.title != null" @click="openLink(item.urllink)">{{
              truncateText(item.title, 12) }}</a>
          </el-col>
        </el-row>
      </div>
    </el-row>




  </div>
</template>

<script>
import MyNav from './nav.vue';
import axios from 'axios';


export default {
  components: {
    'my-nav': MyNav
  },
  data() {
    return {
      VideoCarousel: [],
      VideoRight: [],
      VideoAll: [],
    }
  },

  created() {
    this.getVideoData(3, '通用', '', this.VideoCarousel);
    this.getVideoData(6, '', '', this.VideoRight);
    this.getVideoData(15, '', '', this.VideoAll);
  },

  methods: {
    //打开链接的页面
    openLink(url) {
      window.open(url, '_blank');
    },

    //从后端接口获取数据
    async getVideoData(newsQuantity, tag1, tag2, dataItems) {
      let response
      try {
        const requestData = {
          num: newsQuantity,
          matchTag: String(tag1),
          propertyTag: String(tag2),
        };
        response = await axios.post('/api/Video/GetVideoRandomly', requestData, {
          headers: {
            'Content-Type': 'application/json',
          },
        }); // 发送POST请求，并将请求数据作为 JSON 对象发送
      } catch (error) {
        // console.error(error);
        // 处理获取失败的情况
        this.$message.error('数据获取失败，请重试！');
      }
      console.log(response.data.value);
      // 将数组存贮于传入的数组名中
      // dataItems = response.data.value;
      dataItems.splice(0, dataItems.length, ...response.data.value);
      console.log(this.VideoCarousel);
      return;
    },

    //换一批，即更新数据
    updata() {
      this.getVideoData(6, '', '', this.VideoRight);
      this.getVideoData(15, '', '', this.VideoAll);
    },

    //按照select筛选联赛视频
    selectVideo(selectTag) {
      this.getVideoData(6, String(selectTag), '', this.VideoRight);
      this.getVideoData(15, String(selectTag), '', this.VideoAll);
    },

    //截断过长的文本
    truncateText(text, maxLength) {
      if (text.length > maxLength) {
        return text.substring(0, maxLength) + '...';
      }
      return text;
    },
  },


}

</script>

<style  scoped>
.VideoCarousel {
  position: relative;
  top: 2vh;
  left: 3vw;
}

.VideoRecommend {
  position: relative;
  top: 2vh;
  left: 7vw;
}

.Videocontainer {
  position: relative;
  top: -12vh;
  left: 6vw;
}

.imgItem {
  top: 0vh;
  left: 0%;
  width: 90%;
  height: 90%;
  position: relative;
  border-radius: 6px;
}

.Alldescription {
  position: relative;
  top: 0.5vh;
  left: 0%;
  white-space: nowrap;
  color: #000;
  font-family: Inder;
  font-size: 20px;
  font-style: normal;
  font-weight: 400;
  line-height: normal;

  white-space: nowrap;
  /* 不换行 */
  overflow: hidden;
  /* 隐藏超出容器宽度的文本 */
  text-overflow: ellipsis;
  /* 使用省略号表示超出的文本 */
}

.imgANO {
  left: 0%;
  width: 100%;
  height: 120%;
  position: relative;
}

/* 左上走马灯 */
.Carousel {
  left: 8%;
  width: 100%;
  height: 100%;
  position: relative;
}

/* 走马灯描述 */
.description {
  position: absolute;
  bottom: 0;
  left: 0%;
  width: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  color: white;
  padding: 8px;

  white-space: nowrap;
  /* 不换行 */
  overflow: hidden;
  /* 隐藏超出容器宽度的文本 */
  text-overflow: ellipsis;
  /* 使用省略号表示超出的文本 */
}

.titleLeagueLeft {
  width: 364px;
  height: 76px;
  flex-shrink: 0;
  color: #000;
  font-family: Microsoft YaHei;
  font-size: 36px;
  font-style: normal;
  font-weight: 700;
  line-height: normal;
  position: relative;
  left: 6vw;
  top: -3vh;
}

.line {
  background: #000000;
  box-shadow: 0px 4px 4px 0px rgba(0, 0, 0, 0.25) inset;
  position: relative;
}

.imgRightItem {
  top: 0vh;
  left: 0%;
  width: 80%;
  height: 80%;
  position: relative;
  border-radius: 6px;
}

.Rightdescription {
  position: relative;
  top: 0.5vh;
  left: 0%;
  color: #000;
  font-family: Inder;
  font-size: 15px;
  font-style: normal;
  font-weight: 400;
  line-height: normal;
  white-space: nowrap;
  /* 不换行 */
  overflow: hidden;
  /* 隐藏超出容器宽度的文本 */
  text-overflow: ellipsis;
  /* 使用省略号表示超出的文本 */
}

.Gossipicon {
  position: relative;
  left: -15.5vw;
  top: 7vh;
  font-size: 50px;
  color: rgb(134, 71, 228);
}

.GossipButton {
  position: relative;
  left: -14vw;
  top: 8.5vh;
}

.Shifticon {
  position: relative;
  left: 0.1vw;
  font-size: 20px;
  color: rgb(134, 71, 228);
  transform: rotate(90deg);
}

.shiftButton {
  position: relative;
  left: 88vw;
  top: -34vh;
  background-color: rgb(255, 255, 255);
  border-radius: 2px;
  width: 22px;
  height: 90px;
  border-width: 1px;
  border-style: solid;
  border-color: #dfdddd;
}

.shift {
  position: relative;
  left: 0.2vw;
  font-size: 13px;
  word-wrap: break-word;
  font-weight: 30px;
}
</style>