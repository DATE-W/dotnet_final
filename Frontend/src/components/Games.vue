<!-- 2154314_郑楷_赛事列表 2023.09.07 18:30 v3.3.0
 v1.0.0 页面画了一半 
  v1.1.0 画出了左侧的联赛选择器（未添加逻辑），布局了中部的比赛列表（已添加跳转逻辑），增加了各大联赛LOGO素材图，日期选择器和广告区待实现
  v1.2.0 优化了联赛选择器组件的代码、视觉效果、功能、数据通路
  v1.3.0 中间一列赛事缩略图的代码简化与传值
  v1.4.0 功能基本完成，前后端接口已经对齐，能完成实时渲染
  v1.5.0 增加跳转到赛事详情页的通路，并完成传值，添加注释
  v1.6.0 与赛事详情页互相传值，使得在返回本页面时，之前的日期与联赛选择不会丢失
  v1.7.0 添加显示主队近期三场比赛功能，添加队标显示功能，美化页面
  v1.8.0 调用获取当前用户状态的接口，未登录时跳转到登陆界面 
 v2.0.0 正式版首版，所有功能均已完成，删去了冗余代码
  v2.1.0 优化主队近期赛事板块，在用户无主队时跳转到设置主队界面
  v2.2.0 显示主队按钮优化，bug修复
 v3.0.0 正式版再版，重新调整元素布局，修改所有样式，增加列表的相对页面滚动功能
  v3.1.0 增加中部赛事列表的底线，修改时间选择器可以清除日期的BUG
  v3.2.0 规范代码格式，添加注释
  v3.3.0 细微修改 -->

<template>
  <!-- 顶部部导航栏 -->
  <my-nav></my-nav>
  <!-- 动态标题 -->
  <div class="titleFlex">
    <div class="artTitle" ref="artTitle">
      <a>同济绿茵</a>
    </div>
    <div class="gameTitle" ref="gameTitle">
      <a>&nbsp;&nbsp;&nbsp;赛事</a>
    </div>
  </div>
  <!-- 上部联赛选择器 -->
  <div class="borderBoxLeft topLeague">
    <!-- 使用v-for指令循环生成联赛选择器内容 -->
    <div class="borderBoxLeague" v-for="(league, index) in leagues" :key="index"
      :style="{ left: `${index * 12 + 1.5}rem`, background: ((index == league11) ? 'rgb(255, 160, 187)' : '') }"
      @click="leagueChoice(index)">
      <!-- 插入联赛LOGO图片 -->
      <img v-if="league.logo" :src="league.logo" class="imgLogo">
      <!-- 将top值调整为合适的位置，同时调整“全部赛事”的位置 -->
      <p class="textTypoLeague" :style="{ top: '-1.5rem', left: ((0 == index || 7 == index) ? '1.6rem' : '5.3rem') }">{{
        league.name }}
      </p>
    </div>
  </div>

  <!-- 中上部时间选择器 -->
  <div class="timePickBox">
    <!-- 日期展示 -->
    <span class="textTypoLeague date">当前选择日期: {{ date11 }}</span>
    <!-- 日期选择器 -->
    <el-date-picker v-model="date11" type="date" placeholder="日期选择" :size="large" value-format="YYYY-MM-DD"
      style="left:38.5rem;top:1rem" :clearable="false" :editable="false"
      @change="this.getMatches(this.date11, this.league11);" />
  </div>

  <!-- 中部赛事列表 -->
  <div class="borderBoxMid midList">
    <!-- 当前筛选条件无比赛情况 -->
    <el-empty v-show="!matches.length" description="本日暂无赛事" style="margin-top: 2rem;" />
    <!-- 使用v-for循环生成赛事列表 -->
    <div class="borderBoxMatch" v-for="(match, index) in matches" :key="match.gameUid"
      :style="{ top: `${index * 6 + 0.8}rem` }">
      <!-- 根据matches数据渲染赛事列表的内容 -->
      <!-- 主队队标 -->
      <div class="imgBox">
        <img :src="match.homeLogo">
        <div class="modal2"></div>
      </div>
      <!-- 客队队标 -->
      <div class="imgBox guest">
        <img :src="match.guestLogo">
        <div class="modal2"></div>
      </div>
      <!-- 动态悬浮遮罩层 -->
      <div class="borderBoxMatchModal" @click="toMatchDetail(match.gameUid, this.league11, this.date11)">
        <!-- 比赛开始时间 -->
        <p class="textTypoMatchTime">{{ match.startTime }}</p>
        <!-- 主队名称 -->
        <p class="textTypoMatchTeam">{{ match.homeTeamName }}</p>
        <!-- 比分 -->
        <p class="textTypoMatchScore">{{ scoreCheck(match.homeScore) }} - {{ scoreCheck(match.guestScore) }}</p>
        <!-- 客队名称 -->
        <p class="textTypoMatchTeam guest1">{{ match.guestTeamName }}</p>
        <!-- 比赛状态 -->
        <p class="textTypoMatchStatus">{{ getMatchStatus(match.status) }}</p>
      </div>
    </div>
    <!-- 比赛多时，进行判断，用户拖动到底有提示 -->
    <div class="bottomLine" v-if="(matches.length >= 5)" :style="{ top: `${matches.length * 6 + 2}rem` }">
      <p>我是有底线的噢</p>
    </div>
  </div>

  <!-- 右部主队容器 -->
  <div class="borderBoxRightAD">
    <!-- 三种状态按钮 -->
    <div class="showMainTeam" v-show="mainTeamButton" @click="mainTeamButtonAction">
      <!-- 未登录情况-跳转登陆界面 -->
      <div v-if="!onAccount">
        <p class="textTypoInfo">未检测到账号</p>
        <p class="textTypoInfo">点击此处登录</p>
      </div>
      <!-- 无主队情况-跳转主队选择 -->
      <div v-else-if="this.mainTeam == '暂无主队'">
        <p class="textTypoInfo">目前暂无主队</p>
        <p class="textTypoInfo">点击此处设置</p>
      </div>
      <!-- 有主队情况-显示主队赛果 -->
      <div v-else>
        <p class="textTypoInfo">点击此处查看</p>
        <p class="textTypoInfo">主队近期赛果</p>
      </div>
    </div>
    <!-- 判断是否显示主队赛果 -->
    <div v-show="this.onAccount && this.mainTeam && (!mainTeamButton)">
      <!-- 显示主队名称 -->
      <p class="textTypoLeague mainTeam1">主队: {{ mainTeam }}</p>
      <!-- 使用v-for指令循环生成主队近三场赛果 -->
      <div class="borderBoxRecentMatch" v-for="(recentMatch, index) in recentMatches" :key="recentMatch.gameUid"
        :style="{ top: `${index * 6 + 6.3}rem` }">
        <!-- 对手队标 -->
        <div class="imgBox">
          <img :src="recentMatch.opponentLogo">
          <div class="modal2"></div>
        </div>
        <!-- 赛况 -->
        <div class="borderBoxRecentMatchModal" @click="toMatchDetail(recentMatch.gameUid, this.league11, this.date11)">
          <!-- 主队比分 -->
          <p class="textRecentMatchScore">{{ recentMatch.homeScore }}</p>
          <!-- 对手比分 -->
          <p class="textRecentMatchScore oppScore"> - {{ recentMatch.opponentScore }}</p>
          <!-- 比赛日期 -->
          <p class="gameDateR">{{ recentMatch.gameDate }}</p>
          <!-- 对手队名 -->
          <p class="textVsComponent">VS {{ recentMatch.opponentName }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import MyNav from './nav.vue';
import axios from 'axios';
import { ElMessage } from 'element-plus';
import { ref } from 'vue';
import anime from 'animejs';

export default {

  components: {
    'my-nav': MyNav
  },

  created() {
    if (this.$route.query.date11) {
      this.date11 = this.$route.query.date11;
    }
    this.league11 = this.$route.query.league11;
    this.getMainTeam();
    this.getMatches(this.date11, this.league11);
  },

  mounted() {
    this.animateArtTitle();
    this.animateGameTitle();
  },

  methods: {
    animateArtTitle() {
      const artTitle = this.$refs.artTitle; // 获取标题元素
      artTitle.innerHTML = artTitle.textContent.replace(/\S/g, "<span class='letter'>$&</span>");

      // 使用Anime.js创建动画效果
      anime.timeline({ loop: false })
        .add({
          targets: '.letter',
          scale: [0.3, 1],
          opacity: [0, 1],
          translateZ: 0,
          easing: 'easeOutExpo',
          duration: 600,
          delay: (el, i) => 70 * (i + 1)
        })
    },
    animateGameTitle() {
      const gameTitle = this.$refs.gameTitle; // 获取标题元素
      gameTitle.innerHTML = gameTitle.textContent.replace(/\S/g, "<span class='letter'>$&</span>");

      // 使用Anime.js创建动画效果
      anime.timeline({ loop: false })
        .add({
          targets: '.letter',
          scale: [0.3, 1],
          opacity: [0, 1],
          translateZ: 0,
          easing: 'easeOutExpo',
          duration: 600,
          delay: (el, i) => 70 * (i + 1)
        })
    },
    // 跳转到赛事详情页
    toMatchDetail(uid, leagueC, dateC) {
      //etTimeout(function(){ getSignature() },5000);//Test
      this.$router.push(
        {
          path: `/detailedMatch`,
          query: {
            gameUid: uid,
            lgeChoice: leagueC,
            dateChoice: dateC,
          }
        }
      );
    },
    // 跳转到登陆界面
    redirectToLogin() {
      // 跳转到登录页面的逻辑
      this.$router.push({
        path: '/signin',
        query: {
          isAdmin: 0,
        }
      });
    },
    // 跳转到设置主队
    redirectToEdit() {
      this.$router.push(
        {
          path: '/personalEdit',
          query: {
            value: 'teamedit',
          }
        }
      );
    },
    // 选择联赛
    leagueChoice(choice) {
      if (this.league11 != choice) {
        this.league11 = choice;
      }
      else {
        this.league11 = 0;
      }
      this.getMatches(this.date11, this.league11);
    },
    // 比分空值处理
    scoreCheck(score) {
      if (!score) {
        return 0;
      }
      else {
        return score;
      }
    },
    // 根据赛事状态返回对应的文字描述
    getMatchStatus(status) {
      switch (status) {
        case 'Played':
          return '已结束';
        case 'Unplayed':
          return '未开始';
        case 'Postponed':
          return '比赛延期';
        case 'Playing':
          return '进行中';
        default:
          return '未知';
      }
    },
    // 将日期对象转换为字符串
    dateToString(date) {
      var year = date.getFullYear();
      var month = (date.getMonth() + 1).toString();
      var day = (date.getDate()).toString();
      if (month.length == 1) {
        month = "0" + month;
      }
      if (day.length == 1) {
        day = "0" + day;
      }
      var dateTime = year + "-" + month + "-" + day;
      return dateTime;
    },
    // 主队按钮
    mainTeamButtonAction() {
      this.mainTeamButton = false;
      if (!this.onAccount) {
        this.redirectToLogin();
      }
      else if (this.mainTeam == '暂无主队') {
        this.redirectToEdit();
      }
      else {
        this.getRecentMatches(this.mainTeam);
      }
      return;
    },
    // 调用接口获取赛事列表数据
    async getMatches(dateCho, leagueCho) {
      let response
      try {
        response = await axios.post('/api/updateTeam/searchTeamInGameTime', {
          dateTime: dateCho,
          gameType: leagueCho,
        }, {})
      } catch (err) {
        ElMessage({
          message: '获取赛事数据失败',
          grouping: false,
          type: 'error',
        });
      }
      this.matches = response.data;
      return;
    },
    // 调用接口获取主队近期三场赛事
    async getRecentMatches(mainTeam) {
      let response
      try {
        response = await axios.post('/api/updateTeam/getTeamMatchesByName', {
          teamName: mainTeam,
        }, {})
      } catch (err) {
        ElMessage({
          message: '获取主队近期赛事数据失败',
          grouping: false,
          type: 'error',
        });
      }
      this.recentMatches = response.data;
      return;
    },
    // 调用接口获取主队名
    async getMainTeam() {
      const token = localStorage.getItem('token');
      if (token == null) {
        return;
      }
      this.onAccount = true;
      let response
      try {
        const headers = {
          Authorization: `Bearer ${token}`,
        };
        response = await axios.post('/api/User/profile', {
        }, { headers })
      } catch (err) {
        ElMessage({
          message: '获取主队信息失败',
          grouping: false,
          type: 'error',
        });
      }
      this.mainTeam = response.data.value.uft;
      return;
    },

  },

  data() {
    return {

      league11: 0,
      date11: ref(this.dateToString(new Date())),

      leagues: [
        { name: "全部赛事", logo: "" },
        { name: "英超", logo: "/src/assets/img/pmlogo.png" },
        { name: "西甲", logo: "/src/assets/img/lllogo.png" },
        { name: "意甲", logo: "/src/assets/img/salogo.png" },
        { name: "德甲", logo: "/src/assets/img/bllogo.png" },
        { name: "法甲", logo: "/src/assets/img/le1logo.png" },
        { name: "中超", logo: "/src/assets/img/cslogo.png" },
        /* { name: "其他赛事", logo: "" }, */],

      /* 正式版本 */
      matches: ref([]),
      mainTeam: "",
      recentMatches: ref([]),
      onAccount: false,
      mainTeamButton: true,

    }
  }
}


</script>


<style scoped>
@import '../assets/font/font.css';

/* 左侧容器框 */
.borderBoxLeft {
  position: absolute;
  top: 4.5rem;
  width: 86rem;
  height: 5rem;
  flex-shrink: 0;
  /* 正式版本 */
  background: white;
  /* 测试版本 */
  /* background: rgb(21, 227, 227); */
  border: 2px solid #EAEAEA;
  border-radius: 14px;

  &.topLeague {
    left: 8.5rem;
  }
}

.timePickBox {
  width: 55rem;
  height: 4rem;
  background-color: white;
  top: 12rem;
  left: 14rem;
  position: absolute;
  border: 2px solid #EAEAEA;
  border-radius: 14px;
}

/* 中部容器框 */
.borderBoxMid {
  position: absolute;
  width: 42rem;
  height: 29.5rem;
  flex-shrink: 0;
  /* 正式版本 */
  background: white;
  /* 测试版本 */
  /* background: aqua; */
  overflow-y: auto;
  border: 2px solid #EAEAEA;
  border-radius: 14px;

  &.midList {
    left: 27rem;
    top: 18.5rem;
  }
}

.borderBoxMid::-webkit-scrollbar {
  width: 0;
}

/* 右侧下方容器框 */
.borderBoxRightAD {
  position: absolute;
  width: 19rem;
  height: 25rem;
  left: 72rem;
  flex-shrink: 0;
  top: 12rem;
  /* 正式版本 */

  /* 测试版本 */
  /* background: #4B8FDF; */
}

.imgBox {
  position: absolute;
  left: 5rem;
  width: 10rem;
  height: 4rem;

  &.guest {
    left: 29.4rem;
  }
}

.modal2 {
  position: absolute;
  left: 0rem;
  top: 4rem;
  width: 10rem;
  height: 7rem;
  background-color: white;
  /* background-color: aqua; */
}

/* 联赛选择按钮 */
.borderBoxLeague {
  position: absolute;
  width: 11rem;
  height: 4rem;
  flex-shrink: 0;
  border-radius: 1.5rem;
  border: 1px solid var(--colors-light-eaeaea-100, #EAEAEA);
  top: 0.5rem;
  background-color: #ffffff;
  transition: background-color 0.8s ease;
}

.borderBoxLeague:hover {
  /* background-color: rgb(246, 77, 77); */
  background-color: aqua;
}

.bottomLine {
  position: absolute;
  left: 18.8rem
}

/* 各场赛事框 */
.borderBoxMatch {
  position: absolute;
  width: 39.8rem;
  height: 4rem;
  left: 1rem;
  flex-shrink: 0;
  border-radius: 1.4rem;
  border: 0.05rem solid var(--colors-light-eaeaea-100, #d1d1d1);
  transition: background-color 0.8s ease;
  /* 正式版本 */
  background: white;
}

.borderBoxMatchModal {
  position: absolute;
  width: 39.8rem;
  height: 4rem;
  flex-shrink: 0;
  border-radius: 1.4rem;
  border: 0.05rem solid var(--colors-light-eaeaea-100, #d1d1d1);
  transition: background-color 0.8s ease;
  /* 正式版本 */
  background: rgba(255, 255, 255, 0.7);
}

.borderBoxMatchModal:hover {
  background: rgba(240, 240, 240, 0.9);
}

.showMainTeam {
  position: absolute;
  width: 17rem;
  height: 9rem;
  flex-shrink: 0;
  border: 2px solid #EAEAEA;
  border-radius: 14px;
  background: white;
  display: flex;
  justify-content: center;
}

.borderBoxRecentMatch {
  position: absolute;
  width: 17rem;
  height: 4rem;
  flex-shrink: 0;
  border-radius: 1.4rem;
  border: 0.05rem solid var(--colors-light-eaeaea-100, #d1d1d1);
  transition: background-color 0.8s ease;
  /* 正式版本 */
  background: white;
}

.borderBoxRecentMatchModal {
  position: absolute;
  width: 17rem;
  height: 4rem;
  flex-shrink: 0;
  border-radius: 1.5rem;
  border: 0.05rem solid var(--colors-light-eaeaea-100, #d1d1d1);
  transition: background-color 0.8s ease;
  /* 正式版本 */
  background: rgba(255, 255, 255, 0.7);
}

.borderBoxMatch:hover {
  background-color: rgb(240, 240, 240);
}

.borderBoxRecentMatch:hover {
  background-color: rgb(240, 240, 240);
}

.borderBoxRecentMatchModal:hover {
  background-color: rgba(240, 240, 240, 0.9);
}



/* 字体样式 */

/* 左侧联赛名称字体样式 */
.textTypoLeague {
  position: absolute;
  width: 8rem;
  height: 2rem;
  top: 2rem;
  left: 6rem;
  color: var(--colors-text-dark-172239100, #172239);
  font-feature-settings: 'clig' off, 'liga' off;
  font-family: Verdana;
  font-size: 2rem;
  font-style: normal;
  font-weight: 600;
  line-height: normal;

  &.date {
    top: 0.5rem;
    left: 3rem;
    width: 30rem;
  }

  &.mainTeam1 {
    right: 1rem;
    width: 20rem;
    top: 0rem
  }
}

.textTypoInfo {
  font-family: Verdana;
  font-size: 1.5rem;
  font-style: normal;
  font-weight: 600;
  line-height: normal;
  color: black;
}

.textTypoMatchTime {
  position: absolute;
  color: rgb(17, 60, 158);
  font-family: Georgia;
  font-size: 1.5rem;
  font-style: normal;
  font-weight: 100;
  line-height: normal;
  top: -1rem;
  left: 1rem;
}

.gameDateR {
  position: absolute;
  color: rgb(17, 60, 158);
  font-family: Georgia;
  font-size: 0.9rem;
  font-style: normal;
  font-weight: 100;
  line-height: normal;
  top: -1rem;
  right: 1rem;
}

.textTypoMatchTeam {
  position: absolute;
  color: rgb(0, 0, 0);
  font-family: Impact;
  font-size: 2rem;
  font-style: normal;
  font-weight: 400;
  line-height: normal;
  top: -1.5rem;
  left: 6rem;

  &.guest1 {
    left: auto;
    right: 1rem
  }
}

.textTypoMatchStatus {
  position: absolute;
  color: rgb(97, 97, 97);
  font-family: Impact;
  font-size: 0.8rem;
  font-style: normal;
  font-weight: 200;
  line-height: normal;
  top: 1.6rem;
  left: 1.7rem;
}

.textTypoMatchScore {
  position: absolute;
  color: rgb(255, 0, 0);
  font-family: Verdana;
  font-size: 2.5rem;
  font-style: normal;
  font-weight: 600;
  line-height: normal;
  top: -2.2rem;
  left: 18rem;
}

.textRecentMatchScore {
  position: absolute;
  color: rgb(255, 0, 0);
  font-family: Verdana;
  font-size: 2rem;
  font-style: normal;
  font-weight: 600;
  line-height: normal;
  top: -1.2rem;
  left: 1rem;

  &.oppScore {
    left: 3rem;
    color: black;
  }
}

.textVsComponent {
  position: absolute;
  color: black;
  font-family: Verdana;
  font-size: 1rem;
  font-style: normal;
  font-weight: 400;
  line-height: normal;
  top: 0.4rem;
  left: 7rem;
}

/* 联赛图标 */
.imgLogo {
  position: absolute;
  width: 4rem;
  height: 4rem;
  border-radius: 2rem;
  left: 1rem;
  object-fit: cover;
}

.titleFlex {
  position: absolute;
  top: 24rem;
  left: 8rem;
}

.artTitle {
  font-family: artfont;
  font-size: 50px;
  font-style: normal;
  font-weight: 400;
}

.gameTitle {
  font-family: artfont;
  font-size: 50px;
  font-style: normal;
  font-weight: 400;
}
</style>