<template>
  <div>
    <!-- 顶部导航栏 -->
    <my-nav></my-nav>
    <div>
      <el-row>
        <!-- 新闻（位于左侧） -->
        <el-col :span="18">
          <p class="titleLeagueLeft">足坛八卦</p>
          <el-icon class="Gossipicon">
            <Promotion />
          </el-icon>
          <div class="GossipButton">
            <el-row class="mb-4">
              <el-button plain @click="selectGossip('中超')">中超</el-button>
              <el-button type="primary" plain @click="selectGossip('英超')">英超</el-button>
              <el-button type="success" plain @click="selectGossip('西甲')">西甲</el-button>
              <el-button type="info" plain @click="selectGossip('意甲')">意甲</el-button>
              <el-button type="warning" plain @click="selectGossip('德甲')">德甲</el-button>
              <el-button type="danger" plain @click="selectGossip('法甲')">法甲</el-button>
              <el-button plain @click="selectGossip('')">ALL</el-button>
            </el-row>
          </div>
          <div class="line" style="width: 40vw;height: 0.2px;top:-9vh;left:8%;"></div>
          <div v-if="GossipNews.length != 0" v-for="item in GossipNews" :key="item.id" class="itemSearch">
            <div class="imgWrapper" @click="openNewsDetails(item)">
              <img
                v-if="item.pictureRoutes != null && (matchMP4(item.pictureRoutes[0]) == false || item.newsBody.news_id > 150)"
                referrerPolicy='no-referrer' :src="item.pictureRoutes[0]" alt="Image" class="imgSearch">
              <video
                v-if="item.pictureRoutes != null && matchMP4(item.pictureRoutes[0]) == true && item.newsBody.news_id < 150"
                referrerPolicy='no-referrer' ref="videoPlayer" :src="item.pictureRoutes[0]"
                class="imgSearch imgForVideo" />
            </div>
            <div class="TextWrapper" @click="openNewsDetails(item)">
              <div class="titleSearch">{{ item.newsBody.title }}</div>
              <div class="descriptionSearch">{{ truncateText(item.newsBody.summary, 60) }}</div>
            </div>
          </div>
          <div class="NoMore">No More ......</div>
        </el-col>
        <!-- 视频（位于右侧） -->
        <el-col :span="6">
          <p class="titleRight" style="font-size: 25px;">相关视频</p>
          <el-icon class="iconSearch">
            <VideoPlay />
          </el-icon>
          <div class="line" style="width: 22vw;height: 0.2px;top:-3vh;left:-6%;"></div>
          <div class="imgListSearch">
            <div v-if="GossipNewsVideo.length != 0" v-for="(item, index) in GossipNewsVideo" :key="index"
              class="imgItemSearch">
              <el-row :gutter="20">
                <el-col :span="11">
                  <img referrerPolicy='no-referrer' :src="item.cover" alt="Image" class="imgVideoSearch"
                    @click=" openLink(item.urllink)">
                </el-col>
                <el-col :span="8">
                  <div class="descriptionVideoSearch" @click=" openLink(item.urllink)">{{ truncateText(item.title, 25) }}
                  </div>
                </el-col>
              </el-row>
            </div>
          </div>
          <div class="NoMore" style="top:8vh">No More ......</div>
        </el-col>
      </el-row>
    </div>
  </div>
  <el-backtop :right="100" :bottom="100" />
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
      GossipNews: [],
      GossipNewsVideo: [],
    }
  },

  created() {
    this.getData(-1, '', '八卦', this.GossipNews);
    this.getVideo(-1, '', '八卦', this.GossipNewsVideo);
  },

  methods: {
    //从后端接口获取新闻数据
    async getData(newsQuantity, tag1, tag2, dataItems) {
      try {
        const requestData = {
          num: newsQuantity,
          matchTag: String(tag1),
          propertyTag: String(tag2),
        };

        const response = await axios.post('/api/News/GetNews', requestData, {
          headers: {
            'Content-Type': 'application/json',
          },
        }); // 发送POST请求，并将请求数据作为 JSON 对象发送

        console.log(response.data.value);

        // 将数组存贮于传入的数组名中
        dataItems.splice(0, dataItems.length, ...response.data.value);
        // console.log(dataItems);
        // console.log(this.items);
      } catch (error) {
        // console.error(error);
        // 处理获取失败的情况
        this.$message.error('数据获取失败，请重试！');
      };
      return;
    },

    //从后端接口获取视频数据
    async getVideo(newsQuantity, tag1, tag2, dataItems) {
      try {
        const requestData = {
          num: newsQuantity,
          matchTag: String(tag1),
          propertyTag: String(tag2),
        };

        const response = await axios.post('/api/Video/GetVideoRandomly', requestData, {
          headers: {
            'Content-Type': 'application/json',
          },
        }); // 发送POST请求，并将请求数据作为 JSON 对象发送

        console.log(response.data.value);

        // 将数组存贮于传入的数组名中
        dataItems.splice(0, dataItems.length, ...response.data.value);
        console.log(dataItems);
        // console.log(this.items);
      } catch (error) {
        // console.error(error);
        // 处理获取失败的情况
        this.$message.error('数据获取失败，请重试！');
      };
      return;
    },

    //按照selectTag来筛选数据
    selectGossip(selectTag) {
      this.getData(-1, selectTag, '八卦', this.GossipNews);
      this.getVideo(-1, selectTag, '八卦', this.GossipNewsVideo);
    },

    //截断过长的文本
    truncateText(text, maxLength) {
      if (text.length > maxLength) {
        return text.substring(0, maxLength) + '...';
      }
      return text;
    },

    //打开新闻详情页
    openNewsDetails(item) {
      // console.log("123");
      // console.log(item);
      const queryString = encodeURIComponent(JSON.stringify(item));
      // this.$router.push({ path: '/NewsDetails', query: { data: queryString } });
      const url = `${window.location.origin}/NewsDetails?data=${queryString}`;
      window.open(url, '_blank');
    },


    //打开链接的页面
    openLink(url) {
      window.open(url, '_blank');
    },

    //匹配mp4字符，用于判断视频还是图片的渲染
    matchMP4(str) {
      return str.includes('mp4');
    }
  }
}

</script>

<style  scoped>
.itemSearch {
  display: flex;
  /* align-items: center; */
  margin-bottom: 10px;
  position: relative;
  left: 8%;
  top: -13vh;
  margin-top: 5%;
}

.imgWrapper {
  margin-right: 10px;
  width: 60%;
}

.TextWrapper {
  display: flex;
  flex-direction: column;
  position: relative;
  left: -7vw;
  top: 1vh;
}

/* 图片样式 */
.imgSearch {
  top: 0vh;
  left: 0%;
  width: 25vw;
  height: 30vh;
  position: relative;
}

/* 标题样式 */
.titleSearch {
  font-weight: bold;
  /* 设置容器的宽度 */
  overflow: hidden;
  /* 隐藏超出容器宽度的文本 */
  text-overflow: ellipsis;
  /* 使用省略号表示超出的文本 */
  /* white-space: nowrap; */
  /* white-space: nowrap;  不换行 */
  width: 27vw;
  margin-top: 5px;
  font-size: 30px;
  position: relative;
  left: -12vw;
}

/* 描述样式 */
.descriptionSearch {
  width: 20vw;
  /* 设置容器的宽度 */
  max-height: 120px;
  /* 设置最大高度 */
  overflow: hidden;
  /* 隐藏超出容器宽度的文本 */
  text-overflow: ellipsis;
  /* 使用省略号表示超出的文本 */
  margin-top: 5px;
  font-size: 18px;
  position: relative;
  left: -12vw;
  top: 2vh;
}

.iconSearch {
  font-size: 30px;
  position: relative;
  top: -4vh;
  left: 75%;
}

.imgVideoSearch {
  top: 0vh;
  left: -2%;
  width: 90%;
  height: 90%;
  position: relative;
}

.descriptionVideoSearch {
  flex-shrink: 0;
  color: #000;
  font-family: Mogra;
  font-size: 18px;
  font-style: normal;
  font-weight: 400;
  line-height: normal;
  position: relative;
  left: -20%;
  top: 0vh;
  width: 180%;
  /* 设置容器的宽度 */
  max-height: 80%;
  /* 设置最大高度 */
  overflow: hidden;
  /* 隐藏超出容器宽度的文本 */
  text-overflow: ellipsis;
  /* 使用省略号表示超出的文本 */
}

.NoMore {
  color: #000;
  text-align: center;
  font-family: STSong;
  font-size: 16px;
  font-style: normal;
  font-weight: 400;
  line-height: normal;
  position: relative;
  top: 5%
}

.imgListSearch {
  display: flex;
  flex-direction: column;
  position: relative;
  top: -3vh;
  left: -2vh;
}

.imgItemSearch {
  display: flex;
  align-items: flex-start;
  margin-bottom: 10px;
  position: relative;
  top: 3vh;
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

.Gossipicon {
  position: relative;
  left: 6vw;
  top: -8vh;
  font-size: 50px;
  color: rgb(134, 71, 228);
}

.GossipButton {
  position: relative;
  left: 10vw;
  top: -12vh;
}

/* 右侧八卦标题 */
.titleRight {
  color: #000;
  font-family: Microsoft YaHei;
  font-size: 20px;
  font-style: normal;
  font-weight: 700;
  line-height: normal;
  position: relative;
  top: 2vh;
  left: -6%;
}

.imgForVideo {
  background-image: url("https://img2.baidu.com/it/u=3484181004,3499441771&fm=253&fmt=auto&app=138&f=JPEG?w=889&h=500");
  /* 指定背景图片为一个播放图标 */
  background-size: cover;
  /* 让背景图片适应伪元素的大小 */
}
</style>