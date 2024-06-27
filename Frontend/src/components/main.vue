<template>
  <div>
    <my-nav></my-nav>
    <el-main style="background-color: #b0d5df;">
      <div class="main-container">
        <!-- Scroll to Top Button -->
        <div @click="scrollToTop" class="scroll-to-top-btn">
          <div class="image-container">
            <img src="../assets/img/ToTop.png" alt="Scroll to Top">
          </div>
          <div class="text-overlay">
            <div class="line">{{ line1 }}</div>
            <div class="line">{{ line2 }}</div>
          </div>
        </div>
        <!-- Carousel -->
        <div
          style="display: flex; justify-content: center; align-items: center; height: 76vh; width: 100vw;margin-top: -30px;">
          <el-carousel style="height: 76vh; z-index: 1; width: 86vw; margin: auto; position: relative;" arrow="never">
            <!-- Images to display above the carousel -->
            <div
              style="position: absolute; top: 5%; left: 0; display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
              <img src="../assets/img/home_slogan1.png"
                style="max-width: 600px; width: 50%; margin: 1.333vw; z-index: 2;">
            </div>
            <div
              style="position: absolute; top: 55%; left: 0; display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
              <img src="../assets/img/home_slogan2.png"
                style="max-width: 600px; width: 50%; margin: 1.333vw; z-index: 2;">
            </div>
            <!-- Replace the v-for loop with your custom carousel items -->
            <el-carousel-item v-for="item in 3" :key="item"
              style="width: 86vw; height: 76vh; display: flex; justify-content: center; align-items: center;">
              <img :src="getImageUrl(item)" alt="Carousel Image" class="carousel-image"
                style="border: 0; width: 100%; height: 90%; object-fit: cover; border-radius: 15px;" />
            </el-carousel-item>
          </el-carousel>
        </div>
        <div class="content-wrapper">
          <div class="top-section">
            <!-- 赛事信息筛选菜单 -->
            <el-menu class="Games-menu" default-active="2">
              <div class="menu-wrapper">
                <el-menu-item index="2">
                  <p class="menu-title" @click="selectLeague('中超')">{{ $t('CSL') }}</p>
                </el-menu-item>
                <el-menu-item index="3">
                  <p class="menu-title" @click="selectLeague('英超')">{{ $t('PL') }}</p>
                </el-menu-item>
                <el-menu-item index="4">
                  <p class="menu-title" @click="selectLeague('意甲')">{{ $t('SA') }}</p>
                </el-menu-item>
                <el-menu-item index="5">
                  <p class="menu-title" @click="selectLeague('西甲')">{{ $t('LL') }}</p>
                </el-menu-item>
                <el-menu-item index="6">
                  <p class="menu-title" @click="selectLeague('德甲')">{{ $t('BL') }}</p>
                </el-menu-item>
                <el-menu-item index="7">
                  <p class="menu-title" @click="selectLeague('法甲')">{{ $t('L1') }}</p>
                </el-menu-item>
                <el-menu-item index="8" @click="redirectToGames">
                  <p class="menu-title">{{ $t('MORE') }}</p>
                </el-menu-item>
              </div>
            </el-menu>
            <!-- 赛事信息卡片 -->
            <div v-if="selectedLeague === '中超' || selectedLeague === ''">
              <el-row class="Game-col-container">
                <el-card shadow="hover" class="game-card" v-for="(game, index) in ZCgame_id" :key="game.id"
                  style="border-radius: 0.3vw; border: none; margin: 0.667vw; background-color: #f1f0ed;"
                  @click="goToGameDetail(ZCgame_id[index])">
                  <div class="game-content">
                    <div class="game-header">
                      <!-- 时间在左上方 -->
                      <div class="column-time">{{ ZCgame_time[index] }}</div>
                      <!-- 比赛状态在右上方 -->
                      <div class="column-status">
                        {{ ZCgame_status[index] === 'Played' ? '已结束' : ZCgame_status[index] }}
                      </div>
                    </div>
                    <div>
                      <div class="game-row">
                        <div class="team-cell">
                          <img :src="ZCteam1_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ ZCteam1_name[index] }}</div>
                        <div class="team-cell">{{ ZCteam1_score[index] }}</div>
                      </div>
                      <div class="game-row-2">
                        <div class="team-cell">
                          <img :src="ZCteam2_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ ZCteam2_name[index] }}</div>
                        <div class="team-cell">{{ ZCteam2_score[index] }}</div>
                      </div>
                    </div>
                  </div>
                </el-card>
              </el-row>
            </div>
            <div v-if="selectedLeague === '英超'">
              <el-row class="Game-col-container">
                <el-card shadow="hover" class="game-card" v-for="(game, index) in YCgame_id" :key="game.id"
                  style="border-radius: 0.3vw; border: none; margin: 0.667vw;background-color: #f1f0ed;"
                  @click="goToGameDetail(YCgame_id[index])">
                  <div class="game-content">
                    <div class="game-header">
                      <!-- 时间在左上方 -->
                      <div class="column-time">{{ YCgame_time[index] }}</div>
                      <!-- 比赛状态在右上方 -->
                      <div class="column-status">
                        {{ YCgame_status[index] === 'Played' ? '已结束' : YCgame_status[index] }}
                      </div>
                    </div>
                    <div>
                      <div class="game-row">
                        <div class="team-cell">
                          <img :src="YCteam1_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ YCteam1_name[index] }}</div>
                        <div class="team-cell">{{ YCteam1_score[index] }}</div>
                      </div>
                      <div class="game-row-2">
                        <div class="team-cell">
                          <img :src="YCteam2_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ YCteam2_name[index] }}</div>
                        <div class="team-cell">{{ YCteam2_score[index] }}</div>
                      </div>
                    </div>
                  </div>
                </el-card>
              </el-row>
            </div>
            <div v-if="selectedLeague === '意甲'">
              <el-row class="Game-col-container">
                <el-card shadow="hover" class="game-card" v-for="(game, index) in YJgame_id" :key="game.id"
                  style="border-radius: 0.3vw; border: none; margin: 0.667vw;background-color: #f1f0ed;"
                  @click="goToGameDetail(YJgame_id[index])">
                  <div class="game-content">
                    <div class="game-header">
                      <!-- 时间在左上方 -->
                      <div class="column-time">{{ YJgame_time[index] }}</div>
                      <!-- 比赛状态在右上方 -->
                      <div class="column-status">
                        {{ YJgame_status[index] === 'Played' ? '已结束' : YJgame_status[index] }}
                      </div>
                    </div>
                    <div>
                      <div class="game-row">
                        <div class="team-cell">
                          <img :src="YJteam1_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ YJteam1_name[index] }}</div>
                        <div class="team-cell">{{ YJteam1_score[index] }}</div>
                      </div>
                      <div class="game-row-2">
                        <div class="team-cell">
                          <img :src="YJteam2_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ YJteam2_name[index] }}</div>
                        <div class="team-cell">{{ YJteam2_score[index] }}</div>
                      </div>
                    </div>
                  </div>
                </el-card>
              </el-row>
            </div>
            <div v-if="selectedLeague === '西甲'">
              <el-row class="Game-col-container">
                <el-card shadow="hover" class="game-card" v-for="(game, index) in XJgame_id" :key="game.id"
                  style="border-radius: 0.3vw; border: none; margin: 0.667vw;background-color: #f1f0ed;"
                  @click="goToGameDetail(XJgame_id[index])">
                  <div class="game-content">
                    <div class="game-header">
                      <!-- 时间在左上方 -->
                      <div class="column-time">{{ XJgame_time[index] }}</div>
                      <!-- 比赛状态在右上方 -->
                      <div class="column-status">
                        {{ XJgame_status[index] === 'Played' ? '已结束' : XJgame_status[index] }}
                      </div>
                    </div>
                    <div>
                      <div class="game-row">
                        <div class="team-cell">
                          <img :src="XJteam1_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ XJteam1_name[index] }}</div>
                        <div class="team-cell">{{ XJteam1_score[index] }}</div>
                      </div>
                      <div class="game-row-2">
                        <div class="team-cell">
                          <img :src="XJteam2_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ XJteam2_name[index] }}</div>
                        <div class="team-cell">{{ XJteam2_score[index] }}</div>
                      </div>
                    </div>
                  </div>
                </el-card>
              </el-row>
            </div>
            <div v-if="selectedLeague === '德甲'">
              <el-row class="Game-col-container">
                <el-card shadow="hover" class="game-card" v-for="(game, index) in DJgame_id" :key="game.id"
                  style="border-radius: 0.3vw; border: none; margin: 0.667vw;background-color: #f1f0ed;"
                  @click="goToGameDetail(DJgame_id[index])">
                  <div class="game-content">
                    <div class="game-header">
                      <!-- 时间在左上方 -->
                      <div class="column-time">{{ DJgame_time[index] }}</div>
                      <!-- 比赛状态在右上方 -->
                      <div class="column-status">
                        {{ DJgame_status[index] === 'Played' ? '已结束' : DJgame_status[index] }}
                      </div>
                    </div>
                    <div>
                      <div class="game-row">
                        <div class="team-cell">
                          <img :src="DJteam1_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ DJteam1_name[index] }}</div>
                        <div class="team-cell">{{ DJteam1_score[index] }}</div>
                      </div>
                      <div class="game-row-2">
                        <div class="team-cell">
                          <img :src="DJteam2_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ DJteam2_name[index] }}</div>
                        <div class="team-cell">{{ DJteam2_score[index] }}</div>
                      </div>
                    </div>
                  </div>
                </el-card>
              </el-row>
            </div>
            <div v-if="selectedLeague === '法甲'">
              <el-row class="Game-col-container">
                <el-card shadow="hover" class="game-card" v-for="(game, index) in FJgame_id" :key="game.id"
                  style="border-radius: 0.3vw; border: none; margin: 0.667vw;background-color: #f1f0ed;"
                  @click="goToGameDetail(FJgame_id[index])">
                  <div class="game-content">
                    <div class="game-header">
                      <!-- 时间在左上方 -->
                      <div class="column-time">{{ FJgame_time[index] }}</div>
                      <!-- 比赛状态在右上方 -->
                      <div class="column-status">
                        {{ FJgame_status[index] === 'Played' ? '已结束' : FJgame_status[index] }}
                      </div>
                    </div>
                    <div>
                      <div class="game-row">
                        <div class="team-cell">
                          <img :src="FJteam1_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ FJteam1_name[index] }}</div>
                        <div class="team-cell">{{ FJteam1_score[index] }}</div>
                      </div>
                      <div class="game-row-2">
                        <div class="team-cell">
                          <img :src="FJteam2_logo[index]" alt="Logo Image" class="logo-image">
                        </div>
                        <div class="team-cell">{{ FJteam2_name[index] }}</div>
                        <div class="team-cell">{{ FJteam2_score[index] }}</div>
                      </div>
                    </div>
                  </div>
                </el-card>
              </el-row>
            </div>
          </div>
        </div>

        <!-- 下中半部分 -->
        <div class="bottom-middle-section">
          <!-- 左侧新闻板块 -->
          <div class="news-container">
            <div class="hot-news">{{ $t('TOPNEWS') }}</div>
            <div class="news-row">
              <div v-for="(news, index) in dataItems" :key="index" class="news-item"
                :class="{ 'two-columns': (index + 2) % 5 === 0 }">
                <a :href="news.link" target="_blank" class="news-link">
                  <el-row align="middle" @click="openNewsDetails(news)">
                    <el-col :span="24" style="display: flex; align-items: center;">
                      <div class="news-item-wrapper" style="position: relative;">
                        <img :src="news.pictureRoutes[0]" alt="News Image" class="news-image"
                          :class="{ 'news-image2': (index + 2) % 5 === 0 }">
                        <div class="news-content-overlay" v-if="(index + 2) % 5 === 0"
                          style="position: absolute; top: 70%; left: 0; width: 32vw; text-align: center; color: white; background-color: rgba(0, 0, 0, 0.281); padding: 1.333vw;">
                          <p class="news-summary">{{ truncateText(news.newsBody.title, 20) }}</p>
                        </div>
                        <div v-else class="news-content" style="text-align: center;">
                          <p class="news-summary">{{ truncateText(news.newsBody.title, 26) }}</p>
                        </div>
                      </div>
                    </el-col>
                  </el-row>
                </a>
              </div>
            </div>
          </div>

          <!-- 添加一个缝隙 -->
          <div class="spacer" style="width: 4vw;"></div>

          <!-- 右侧社区板块 -->
          <div class="forum-container">
            <div class="hot-posts">{{ $t('HOTPOSTS') }}</div>
            <div class="posts-column">
              <div v-for="(post, index) in post_id" :key="post.id" class="posts-item">
                <el-row align="middle" class="posts-row" @click="goToDetail(post_id[index])">
                  <el-col :span="24" style="display: flex; align-items: center;height: 8vw;">
                    <div class="posts-item-wrapper">
                      <!-- News Content -->
                      <div class="posts-content">
                        <p :class="['posts-summary', { 'hovered-summary': hoveredIndex === index }]"
                          @mouseenter="hoveredIndex = index" @mouseleave="hoveredIndex = -1">
                          <span :class="['post-number', getColorClass(index)]">
                            {{ index + 1 }}.
                          </span>
                          {{ truncateText(post_content[index], 50) }}
                        </p>
                        <div class="info-group">
                          <span class="like-container">
                            <el-icon size="medium">
                              <Pointer />
                            </el-icon>
                            <span class="like-number">{{ post_likes[index] }}</span>
                          </span>
                          <span class="space-between"></span> <!-- Add a spacer with desired gap -->
                          <span class="star-container">
                            <el-icon size="medium">
                              <Star />
                            </el-icon>
                            <span class="star-number">{{ post_stars[index] }}</span>
                          </span>
                        </div>
                      </div>
                    </div>
                  </el-col>
                </el-row>
              </div>
            </div>
          </div>
        </div>

        <!-- 下半部分 -->
        <div class="bottom-section">
          <div class="module" @click="redirectToNews" @mouseover="hideContent" @mouseout="showContent">
            <p class="module-text">致 力 于 分 享<br>最 有 价 值 的 新闻</p>
            <el-icon class="news-icon">
              <VideoCamera />
            </el-icon>
            <div class="overlay-background">
              <p class="overlay-text">一览足球热讯<br>->新闻</p>
            </div>
          </div>

          <div class="module" @click="redirectToGames" @mouseover="hideContent" @mouseout="showContent">
            <p class="module-text">一 场 场 足 球<br>视 觉 盛 宴</p>
            <el-icon class="football-icon">
              <Football />
            </el-icon>
            <div class="overlay-background">
              <p class="overlay-text">享受足球魅力<br>->赛事</p>
            </div>
          </div>

          <div class="module" @click="redirectToForum" @mouseover="hideContent" @mouseout="showContent">
            <p class="module-text">一 个 充 满 热 情<br>的 思 想 空 间</p>
            <el-icon class="posts-icon">
              <ChatDotSquare />
            </el-icon>
            <div class="overlay-background">
              <p class="overlay-text">表达真挚热爱<br>->论坛</p>
            </div>
          </div>
        </div>
      </div>
    </el-main>
  </div>
</template>
  
<script>
import MyNav from './nav.vue';
import axios from 'axios';
import { ElIcon, ElMessage } from 'element-plus';
import carousel1 from '../assets/img/home_slider1.jpg';
import carousel2 from '../assets/img/home_slider2.jpg';
import carousel3 from '../assets/img/home_slider3.jpg';


export default {
  data() {
    return {
      selectedLeague: '', // Store the selected league
      hoveredIndex: -1, //帖子内容变色
      line1: '回到',
      line2: '顶部',
      maxGamesItems: 6,
      maxNewsItems: 13,
      maxPostsItems: 10,
      maxNewsLength: 30,
      activeName: 'second',
      pageNumber: 1,
      pageSize: 10,
      totalPosts: 0,
      showPage: false, //初始为false 向后端请求完数据后变为true 更换tag页面暂时变为false
      post_id: [],  //存储返回的帖子id
      post_title: [],  //存储返回的帖子标题
      post_content: [],
      post_likes: [],
      post_stars: [],
      currentTag: 'ALL',  //向后端传递当前页面的帖子类型 初始为全部 不受tag影响
      ZCgame_id: [],
      ZCgame_status: [],
      ZCgame_kind: [],
      ZCteam1_name: [],
      ZCteam2_name: [],
      ZCteam1_logo: [],
      ZCteam2_logo: [],
      ZCteam1_score: [],
      ZCteam2_score: [],
      ZCgame_time: [],

      YCgame_id: [],
      YCgame_status: [],
      YCgame_kind: [],
      YCteam1_name: [],
      YCteam2_name: [],
      YCteam1_logo: [],
      YCteam2_logo: [],
      YCteam1_score: [],
      YCteam2_score: [],
      YCgame_time: [],

      YJgame_id: [],
      YJgame_status: [],
      YJgame_kind: [],
      YJteam1_name: [],
      YJteam2_name: [],
      YJteam1_logo: [],
      YJteam2_logo: [],
      YJteam1_score: [],
      YJteam2_score: [],
      YJgame_time: [],

      DJgame_id: [],
      DJgame_status: [],
      DJgame_kind: [],
      DJteam1_name: [],
      DJteam2_name: [],
      DJteam1_logo: [],
      DJteam2_logo: [],
      DJteam1_score: [],
      DJteam2_score: [],
      DJgame_time: [],

      XJgame_id: [],
      XJgame_status: [],
      XJgame_kind: [],
      XJteam1_name: [],
      XJteam2_name: [],
      XJteam1_logo: [],
      XJteam2_logo: [],
      XJteam1_score: [],
      XJteam2_score: [],
      XJgame_time: [],

      FJgame_id: [],
      FJgame_status: [],
      FJgame_kind: [],
      FJteam1_name: [],
      FJteam2_name: [],
      FJteam1_logo: [],
      FJteam2_logo: [],
      FJteam1_score: [],
      FJteam2_score: [],
      FJgame_time: [],

      news_id: [],
      news_title: [],
      news_content: [],
      news_picture: [],
      news_num: 13,
      dataItems: [],
      matchTag: "",
      propertyTag: "",
    }
  },
  components: {
    'my-nav': MyNav
  },
  created() {
    this.getData(13, '', '', this.dataItems);
  },
  mounted() {
    this.getPosts(1, this.pageSize, this.currentTag);
    this.getRecentGames();
  },
  methods: {
    selectLeague(league) {
      this.selectedLeague = league; // Update the selected league
    },
    //打开新闻详情页
    openNewsDetails(item) {
      console.log(item);
      const queryString = encodeURIComponent(JSON.stringify(item));
      // this.$router.push({ path: '/NewsDetails', query: { data: queryString } });
      const url = `${window.location.origin}/NewsDetails?data=${queryString}`;
      window.open(url, '_blank');
    },
    goToGameDetail(gameId) {
      this.$router.push({
        path: '/detailedMatch',
        query: { gameUid: gameId }
      });
    },
    goToDetail(postId) {
      this.$router.push({
        path: '/detail',
        query: { clickedPostID: postId }
      });
    },
    //从后端获取新闻数据
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

        this.ok = response.data.ok;
        // 将数组存贮于传入的数组名中
        dataItems.splice(0, dataItems.length, ...response.data.value);
        console.log(dataItems);
        // console.log(this.items);
      } catch (error) {
        console.error(error);
        return
      }
      return
    },
    async getRecentGames() {
      let response
      try {
        response = await axios({
          method: 'GET',
          url: '/api/updateTeam/showRecentGames',
        })
      }
      catch (err) {
        ElMessage({
          message: '获取最近赛事信息失败',
          grouping: false,
          type: 'error',
        });
        return
      }
      //console.log(response);
      if (Array.isArray(response.data)) {
        response.data.forEach((postInfo) => {
          if (postInfo.gameName === "中超") {
            this.ZCgame_status.push(postInfo.status);
            this.ZCgame_kind.push(postInfo.gameName);
            this.ZCgame_id.push(postInfo.gameUid);
            this.ZCteam1_name.push(postInfo.homeTeamName);
            this.ZCteam2_name.push(postInfo.guestTeamName);
            this.ZCteam1_logo.push(postInfo.homeTeamLogo);
            this.ZCteam2_logo.push(postInfo.guestTeamLogo);
            this.ZCteam1_score.push(postInfo.homeScore);
            this.ZCteam2_score.push(postInfo.guestScore);
            this.ZCgame_time.push(postInfo.gameTime);
          }
          if (postInfo.gameName === "英超") {
            this.YCgame_status.push(postInfo.status);
            this.YCgame_kind.push(postInfo.gameName);
            this.YCgame_id.push(postInfo.gameUid);
            this.YCteam1_name.push(postInfo.homeTeamName);
            this.YCteam2_name.push(postInfo.guestTeamName);
            this.YCteam1_logo.push(postInfo.homeTeamLogo);
            this.YCteam2_logo.push(postInfo.guestTeamLogo);
            this.YCteam1_score.push(postInfo.homeScore);
            this.YCteam2_score.push(postInfo.guestScore);
            this.YCgame_time.push(postInfo.gameTime);
          }
          if (postInfo.gameName === "意甲") {
            this.YJgame_status.push(postInfo.status);
            this.YJgame_kind.push(postInfo.gameName);
            this.YJgame_id.push(postInfo.gameUid);
            this.YJteam1_name.push(postInfo.homeTeamName);
            this.YJteam2_name.push(postInfo.guestTeamName);
            this.YJteam1_logo.push(postInfo.homeTeamLogo);
            this.YJteam2_logo.push(postInfo.guestTeamLogo);
            this.YJteam1_score.push(postInfo.homeScore);
            this.YJteam2_score.push(postInfo.guestScore);
            this.YJgame_time.push(postInfo.gameTime);
          }
          if (postInfo.gameName === "西甲") {
            this.XJgame_status.push(postInfo.status);
            this.XJgame_kind.push(postInfo.gameName);
            this.XJgame_id.push(postInfo.gameUid);
            this.XJteam1_name.push(postInfo.homeTeamName);
            this.XJteam2_name.push(postInfo.guestTeamName);
            this.XJteam1_logo.push(postInfo.homeTeamLogo);
            this.XJteam2_logo.push(postInfo.guestTeamLogo);
            this.XJteam1_score.push(postInfo.homeScore);
            this.XJteam2_score.push(postInfo.guestScore);
            this.XJgame_time.push(postInfo.gameTime);
          }
          if (postInfo.gameName === "德甲") {
            this.DJgame_status.push(postInfo.status);
            this.DJgame_kind.push(postInfo.gameName);
            this.DJgame_id.push(postInfo.gameUid);
            this.DJteam1_name.push(postInfo.homeTeamName);
            this.DJteam2_name.push(postInfo.guestTeamName);
            this.DJteam1_logo.push(postInfo.homeTeamLogo);
            this.DJteam2_logo.push(postInfo.guestTeamLogo);
            this.DJteam1_score.push(postInfo.homeScore);
            this.DJteam2_score.push(postInfo.guestScore);
            this.DJgame_time.push(postInfo.gameTime);
          }
          if (postInfo.gameName === "法甲") {
            this.FJgame_status.push(postInfo.status);
            this.FJgame_kind.push(postInfo.gameName);
            this.FJgame_id.push(postInfo.gameUid);
            this.FJteam1_name.push(postInfo.homeTeamName);
            this.FJteam2_name.push(postInfo.guestTeamName);
            this.FJteam1_logo.push(postInfo.homeTeamLogo);
            this.FJteam2_logo.push(postInfo.guestTeamLogo);
            this.FJteam1_score.push(postInfo.homeScore);
            this.FJteam2_score.push(postInfo.guestScore);
            this.FJgame_time.push(postInfo.gameTime);
          }
        });
      }
      else {
        ElMessage({
          message: '后端返回的赛事数据格式错误',
          grouping: false,
          type: 'error',
        });
      }
      return
    },
    async getPosts(pageNumber, pageSize, currentTag) {
      let response
      try {
        response = await axios.post('/api/Forum/GetPostbyLike', {
          page: pageNumber,
          count: pageSize,
          tag: String(currentTag),
        }, {})
      } catch (err) {
        ElMessage({
          message: '获取帖子失败',
          grouping: false,
          type: 'error',
        });
        return
      }
      //console.log('response:', response.data);
      this.post_id = [];
      this.post_title = [];
      this.post_content = [];
      this.post_likes = [];
      this.post_stars = [];
      if (response.data.postInfoJsons) {
        response.data.postInfoJsons.forEach((postInfo) => {
          this.post_id.push(postInfo.post_id);
          this.post_title.push(postInfo.title);
          this.post_content.push(postInfo.contains);
          this.post_likes.push(postInfo.approvalNum);
          this.post_stars.push(postInfo.collectNum);
        });
      }
      else {
        ElMessage({
          message: '后端返回的帖子数据格式错误',
          grouping: false,
          type: 'error',
        });
      }

      return
    },
    getColorClass(index) {    //设置热帖序号颜色
      if (index < 3) {
        const colors = ['color-red', 'color-green', 'color-blue']; // Add your desired colors here
        return colors[index];
      } else {
        return 'color-gray'; // Default gray color for other numbers
      }
    },
    scrollToTop() {
      // Scroll to top logic
      window.scrollTo({
        top: 0,
        behavior: "smooth" // Add smooth scrolling animation
      });
    },
    redirectToForum() {
      //跳转到论坛页面的逻辑
      this.$router.push('/forum')
    },
    redirectToGames() {
      //跳转到赛事页面的逻辑
      this.$router.push('/Games')
    },
    redirectToNews() {
      //跳转到新闻页面的逻辑
      this.$router.push('/News')
    },
    getImageUrl(item) {
      switch (item) {
        case 1:
          return carousel1;
        case 2:
          return carousel2;
        case 3:
          return carousel3;
        default:
          return '';
      }
    },
    getLimitedNews() {
      return this.newsList.slice(0, this.maxNewsItems);
    },
    // getLimitedPosts() {
    //   return this.postsList.slice(0, this.maxPostsItems);
    // },
    truncateText(text, limit) {
      if (text.length <= limit) {
        return text;
      } else {
        const truncatedText = text.slice(0, limit);
        return truncatedText + '...';
      }
    },

    redirectToGames() {
      //跳转到个人中心页面的逻辑
      this.$router.push('/Games')
    }
  },
};
</script>

<style scoped>
.main-container {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  /* 垂直方向排列子组件 */
}

.top-section {
  width: 86vw;
  margin-top: 25px;
}

/* 赛事种类选择栏 */
.Games-menu {
  padding: 1.333vw;
  background-color: #f1f0ed;
  border-radius: 30px;
}

.el-menu-item {
  transition: background-color 0.8s ease;
  transition: transform 0.5s ease;
}

.el-menu-item:hover {
  transform: scale(1.15);
  background-color: #b9dec9;
}

.el-menu-item.is-active .menu-title {
  color: #3ba7ea;
}

.menu-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  /* 将菜单项居中显示 */
  max-width: 80vw;
  /* 限定父容器的最大宽度 */
  margin: 0 auto;
  /* 居中显示父容器 */
}

.menu-title {
  font-size: 1.5vw;
  color: #333;
  margin: 0;
  padding: 1.333vw;
  border-radius: 1.067vw;
  cursor: pointer;
}

/* 赛事信息卡片 */
.Game-col-container {
  margin-top: -20px;
  display: flex;
  flex-wrap: nowrap;
  justify-content: space-between;
  padding-top: 1.333vw;
  padding-bottom: 0.333vw;
  /* 防止列折行 */
}

.content-wrapper {
  background-color: #ede3e7;
  border-radius: 10px;
  margin: 10px;
}

.game-card {
  width: 725px;
  height: 150px;
}

.game-card:hover {
  cursor: pointer;
}

.game-content {
  text-align: center;
}

.game-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.column-time {
  font-size: 0.9rem;
}

.column-status {
  font-size: 0.9rem;
}

.game-row {
  display: grid;
  grid-template-columns: 1fr 2.5fr 1fr;
  align-items: center;
}

.game-row-2 {
  margin-top: 8px;
  display: grid;
  grid-template-columns: 1fr 2.5fr 1fr;
  align-items: center;
}

.team-cell {
  text-align: center;
}

.logo-image {
  width: 30px;
  height: 30px;
  border-radius: 50%;
}

.bottom-middle-section {
  display: flex;
  padding-top: 1.333vw;
  justify-content: space-between;
  width: 86vw;
  align-items: flex-start;
  /* Align items at the top */
}

/* 论坛模块 */
/* Center the forum container and set some spacing */
.forum-container {
  width: 40%;
  /* display: flex; */
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  flex-wrap: wrap;
  justify-content: center;
  gap: 2.667vw;
  margin-top: 2.667vw;
  overflow: hidden;
  /* Hide overflow content */
}

.posts-column {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: flex-start;
}

.posts-list {
  display: flex;
  flex-direction: column;
  gap: 2.667vw;
}

.post-number {
  font-weight: bold;
  margin-right: 0.667vw;
}

.color-red {
  color: #e74c3c;
  /* Your desired color for the first number */
}

.color-green {
  color: #27ae60;
  /* Your desired color for the second number */
}

.color-blue {
  color: #3498db;
  /* Your desired color for the third number */
}

.color-gray {
  color: #8e8e8e;
  /* Default gray color for other numbers */
}


.posts-item {
  margin-bottom: 1.333vw;
  width: 100%;
  border: 1px solid #ccc;
  background-color: #fff;
  transition: transform 0.3s, box-shadow 0.3s;
  cursor: pointer;
  border-radius: 1.333vw;
}

.posts-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.posts-link {
  display: flex;
  flex-direction: column;
  height: 100%;
  text-decoration: none;
  color: #333;
}

.posts-item-wrapper {
  display: flex;
  align-items: stretch;
  padding: 1.333vw;
}

.posts-content {
  flex-grow: 1;
}

.posts-title {
  font-size: 18px;
  margin: 0;
}

.posts-summary {
  margin: 10px 0;
  line-height: 1.4;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
}

.posts-summary.hovered-summary {
  color: #3498db;
  /* Light blue color when hovered */
}

.info-group {
  display: flex;
  align-items: center;
  margin-top: 10px;
}

.space-between {
  width: 2.667vw;
}

.like-container,
.star-container {
  display: flex;
  /* Display icon and number in the same line */
  align-items: center;
  gap: 5px;
  /* Adjust the gap between icon and number */
}

.like-number,
.star-number {
  font-size: 14px;
  /* Adjust the font size as needed */
}

.hot-posts {
  font-size: 3.5vw;
  font-weight: bold;
  color: #2f2f35;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.1);
  text-align: center;
  display: flex;
  justify-content: center;
  align-items: center;
}

/* 新闻模块 */

.news-container {
  width: 60%;
  display: flex;
  flex-wrap: wrap;
  gap: 2.667vw;
  margin-top: 2.667vw;
  justify-content: center;
}

.hot-news {
  font-size: 3.5vw;
  font-weight: bold;
  color: #2f2f35;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.1);
  text-align: center;
  display: flex;
  justify-content: center;
  align-items: center;
}


.news-row {
  margin-top: 1.333vw;
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  gap: 1.333vw;
}

.news-item {
  width: 15vw;
  height: 50vh;
  border: 1px solid #ccc;
  overflow: hidden;
  background-color: #fff;
  transition: transform 0.3s, box-shadow 0.3s;
  cursor: pointer;
  margin-bottom: 1.333vw;
  border-radius: 1.333vw;
  box-sizing: border-box;
}

.news-item.two-columns {
  width: 32vw;
}

.news-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.news-link {
  display: flex;
  flex-direction: column;
  height: 100%;
  text-decoration: none;
  color: #333;
}

.news-item-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 100%;
}

.news-content {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1.333vw;
  height: 100%;
}

.news-image-wrapper {
  width: 100%;
  padding: 1.333vw;
}

.news-image {
  width: 15vw;
  height: 25vh;
  margin-bottom: 1.333vw;
}

.news-image2 {
  width: 32vw;
  height: 50vh;
}

.news-summary {
  margin: 10px 0;
  line-height: 1.4;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  /* Number of lines to show */
  -webkit-box-orient: vertical;
}

/* 右下组件——回到顶部 */
.scroll-to-top-btn {
  position: fixed;
  bottom: 20px;
  right: 20px;
  width: 50px;
  height: 50px;
  cursor: pointer;
  overflow: hidden;
  z-index: 999;
}

.image-container img {
  width: 100%;
  height: 100%;
  display: block;
  transition: opacity 0.3s;
}

.text-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  opacity: 0;
  /* Hide the overlay by default */
  pointer-events: none;
  /* Prevent overlay from blocking clicks */
  transition: opacity 0.3s;
}

.scroll-to-top-btn:hover .image-container img {
  opacity: 0;
  /* Hide the image on hover */
}

.scroll-to-top-btn:hover .text-overlay {
  opacity: 1;
  /* Show the overlay on hover */
}

.line {
  color: #4dcdf0;
  font-size: 16px;
}

/* <!-- 下半部分 --> */
/* 赛事，论坛，新闻选择 */
.bottom-section {
  width: 86vw;
  display: flex;
  justify-content: space-around;
  padding: 20px;
}

/* 设置模块样式 */
.module {
  position: relative;
  text-align: center;
  padding: 20px;
  background-color: #f8f4ed;
  border: 1px solid #ccc;
  border-radius: 10px;
  width: 30%;
  height: 120px;
  cursor: pointer;
  /* 添加交互：将鼠标光标变为手型 */
  transition: transform 0.3s ease;
}

.module:hover {
  transform: scale(1.05);
}

.module .module-text {
  font-size: 18px;
  color: rgba(0, 123, 255, 0.7);
  margin-top: 1%;
}

.news-icon,
.posts-icon,
.football-icon {
  font-size: 40px;
  /* Adjust the size of the icon as needed */
  margin-bottom: 10px;
  /* Add margin between icon and text */
  color: rgba(0, 123, 255, 0.7);
}


.overlay-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 123, 255, 0.516);
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 10px;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.module:hover .overlay-background {
  opacity: 1;
}

.module .overlay-text {
  font-size: 24px;
  color: #fff;
}

.module .module-text,
.module .football-icon,
.module .news-icon,
.module .posts-icon {
  visibility: visible;
}

.module:hover .module-text,
.module:hover .football-icon,
.module:hover .news-icon,
.module:hover .posts-icon {
  visibility: hidden;
}
</style>


<!-- <button @click="this.$router.push('/signin')">登录</button>
<button @click="this.$router.push('/signup')">注册</button> -->
