<!-- 2154314_郑楷_赛事详情页 2023.09.07 19:00 v2.0.0 
 v1.0.0 添加页面，改路由，完成赛事列表向该页的UID传值 
 v1.1.0 向赛事列表主页面传值，使得之前的选择结果得到保存
 v1.2.0 画出基本页面功能，增加往球队详情页的跳转逻辑，增加往其他场次的跳转逻辑
 v1.3.0 通过Uid来调用接口，实时渲染页面内容，增加各场比赛之间的直接跳转功能
 v1.4.0 修复了返回比赛一级页面时不能保留原有选择的bug，美化了队标显示
 v1.5.0 直播入口跳转功能，页面美化 
v2.0.0 正式版首版，删去非必要代码，优化格式，添加注释 -->

<template>
  <!-- 顶部导航栏 -->
  <my-nav></my-nav>

  <!-- 返回赛事总览界面的按钮 -->
  <el-button size="large" type="info" icon="back" @click="toGames(this.dateChoice, this.lgeChoice)">Back</el-button>

  <!-- 顶中部联赛时间状态信息框 -->
  <div class="topTextBox">
    <p class="textDate">{{ this.match.dateTime }}&nbsp&nbsp{{ this.match.startTime }}</p>
    <p class="textStatus">{{ getMatchStatus(this.match.status) }}</p>
    <p class="textLeague">{{ getLeagueInfo(this.match.leagueType) }}</p>
  </div>

  <!-- 顶左部主队信息框 -->
  <div class="topTextBoxTRA leftBox">
    <img class="picTeam" :src=this.match.homeLogo style="left:2rem">
    <div class="modal1"></div>
    <div class="topTextBoxTRA1" style="width:20rem" @click="toTeams(this.match.homeTeamName)">
      <p class="textTeamName" style="left:1rem">{{ this.match.homeTeamName }}</p>
    </div>
  </div>

  <!-- 顶右部客队信息框 -->
  <div class="topTextBoxTRA rightBox">
    <img class="picTeam" :src=this.match.guestLogo style="right:2rem">
    <div class="modal1"></div>
    <div class="topTextBoxTRA1" style="width:20rem" @click="toTeams(this.match.guestTeamName)">
      <p class="textTeamName" style="right:1rem">{{ this.match.guestTeamName }}</p>
    </div>
  </div>

  <!-- 顶左部主队得分 -->
  <div class="scoreBox" style="left: 35rem;top:12rem">
    <p class="textScore">{{ scoreCheck(this.match.homeScore) }}</p>
  </div>

  <!-- 顶右部客队得分 -->
  <div class="scoreBox" style="left: 65rem;top:12rem">
    <p class="textScore">{{ scoreCheck(this.match.guestScore) }}</p>
  </div>

  <!-- 标题 -->
  <p class="recentGameTypo" style="left:11rem">近期比赛</p>
  <p class="recentGameTypo" style="left:89rem">近期比赛</p>


  <div class="recentGameBoxLeft" v-for="(homeRecentGame, index) in match.homeRecentGames" :key="homeRecentGame.gameUid"
    :style="{ top: `${index * 7 + 26}rem` }" style="left:11rem;">
    <div class="imgBox" style="left:9rem;">
      <img :src="homeRecentGame.opponentLogo">
      <div class="modal2"></div>
    </div>
    <div class="recentGameBoxLeft1" @click="this.getMatchByUid(homeRecentGame.gameUid);">
      <p class="textMatchDate">{{ homeRecentGame.gameDate }}</p>
      <p class="textMatchScore" style="left:5.5rem">{{ homeRecentGame.homeScore }}&nbsp:&nbsp{{
        homeRecentGame.opponentScore
      }}</p>
      <p class="textMatchOpp" style="right:0.5rem">{{ homeRecentGame.opponentName }}</p>
    </div>
  </div>

  <!-- 中右部v-for生成客队近期三场赛事 -->
  <div class="recentGameBoxLeft" v-for="(guestRecentGame, index) in match.guestRecentGames" :key="guestRecentGame.gameUid"
    :style="{ top: `${index * 7 + 26}rem` }" style="left:74rem;">
    <div class="imgBox" style="right:9rem;">
      <img :src="guestRecentGame.opponentLogo">
      <div class="modal2"></div>
    </div>
    <div class="recentGameBoxLeft1" @click="this.getMatchByUid(guestRecentGame.gameUid);">
      <p class="textMatchDate" style="right:0">{{ guestRecentGame.gameDate }}</p>
      <p class="textMatchScore" style="right:5.5rem">{{ guestRecentGame.opponentScore }}&nbsp:&nbsp{{
        guestRecentGame.homeScore }}</p>
      <p class="textMatchOpp" style="left:0.5rem">{{ guestRecentGame.opponentName }}</p>
    </div>
  </div>

  <!-- 中部直播入口 -->
  <div>
    <img class="intoLiveImg" src="../assets/img/IntoLive.jpg">
    <div class="intoLiveModal" @click="goToVideo(this.match.liveStream);">
      <img class="LiveLogo" src="../assets/img/LiveStreamLogo.png">
      <p class="textLive">直播入口</p>
    </div>
  </div>
</template>

<script>
import MyNav from './nav.vue';
import axios from 'axios';
import { ref } from 'vue';

export default {
  components: {
    'my-nav': MyNav
  },

  created() {
    this.gameUid = this.$route.query.gameUid;
    this.dateChoice = this.$route.query.dateChoice;
    this.lgeChoice = this.$route.query.lgeChoice;
    console.log(this.gameUid);
  },

  mounted() {
    this.getMatchByUid(this.gameUid);
  },

  methods: {
    // 调用接口获取赛事详情数据
    async getMatchByUid(gameUidThis) {
      let response
      try {
        response = await axios.post('/api/updateTeam/getGameByUid', {
          gameUid: gameUidThis,
        }, {})
      } catch (err) {
        ElMessage({
          message: '获取赛事数据失败',
          grouping: false,
          type: 'error',
        });
      }
      /* console.log(dateCho,leagueCho); */
      /* this.homeTeamName = response.data.homeTeamName; */
      this.match = response.data;
      console.log(this.match);
    },
    goToVideo(video_url) {
      window.open(video_url);
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
    toGames(dateChoice, lgeChoice) {
      this.$router.push(
        {
          path: `/Games`,
          query: {
            date11: dateChoice,
            league11: lgeChoice,
          }
        }
      );
    },
    toTeams(teamName11) {
      this.$router.push(
        {
          path: `/teamMsg`,
          query: {
            teamName: teamName11,
          }
        }
      );
    },
    toMatchDetail(uid, leagueC, dateC) {
      this.match11 = uid;
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
    getLeagueInfo(status) {
      switch (status) {
        case 0:
          return "";
        case 1:
          return "英格兰足球超级联赛";
        case 2:
          return "西班牙足球甲级联赛";
        case 3:
          return "意大利足球甲级联赛";
        case 4:
          return "德国足球甲级联赛";
        case 5:
          return "法国足球甲级联赛";
        case 6:
          return "中国足球超级联赛";
        case 7:
          return "";
        default:
          return "";
      }
    },
    getMatchStatus(status) {
      switch (status) {
        case 'Played':
          return "已结束";
        case 'Unplayed':
          return "未开始";
        case 'Playing':
          return "进行中";
        case 'Postponed':
          return "比赛延期";
        default:
          return "待定";
      }
    },
  },

  data() {
    return {
      lgeChoice: 0,
      dateChoice: ref(''),
      match: ref([]),
    }

  }
}
</script>

<style scoped>
/* 框体风格 */
.topTextBox {
  position: absolute;
  width: 27rem;
  height: 10rem;
  flex-shrink: 0;
  background: white;
  border: 2px solid var(--colors-light-eaeaea-100, #EAEAEA);
  border-radius: 1rem;
  left: 39rem;
  top: 9rem;
}

.topTextBoxTRA {
  position: absolute;
  width: 27rem;
  height: 10rem;
  flex-shrink: 0;
  background: white;
  border: 2px solid var(--colors-light-eaeaea-100, #EAEAEA);
  border-radius: 1rem;
  transition: background-color 0.5s ease;

  &.leftBox {
    left: 17rem;
    top: 9rem;
    width: 20rem;
  }

  &.rightBox {
    left: 68rem;
    top: 9rem;
    width: 20rem;
  }
}

.topTextBoxTRA:hover {
  background-color: rgb(237, 224, 224);
}

.topTextBoxTRA1 {
  position: absolute;
  width: 27rem;
  height: 10rem;
  flex-shrink: 0;
  background: rgba(255, 255, 255, 0.7);
  border: 2px solid var(--colors-light-eaeaea-100, #EAEAEA);
  border-radius: 1rem;
  transition: background-color 0.5s ease;
}

.topTextBoxTRA1:hover {
  background-color: rgba(237, 224, 224, 0.9);
}

.scoreBox {
  position: absolute;
  width: 5rem;
  height: 5rem;
  flex-shrink: 0;
  background: white;
  border: 2px solid var(--colors-light-eaeaea-100, #EAEAEA);
  border-radius: 1rem;
  text-align: center;

}

.recentGameBoxLeft {
  position: absolute;
  width: 20rem;
  height: 5rem;
  flex-shrink: 0;
  background: white;
  border: 2px solid var(--colors-light-eaeaea-100, #EAEAEA);
  border-radius: 1rem;
  transition: background-color 0.5s ease;
}

.recentGameBoxLeft1 {
  position: absolute;
  width: 20rem;
  height: 5rem;
  flex-shrink: 0;
  background: rgba(255, 255, 255, 0.7);
  border: 2px solid var(--colors-light-eaeaea-100, #EAEAEA);
  border-radius: 1rem;
  transition: background-color 0.5s ease;
}

.recentGameBoxLeft:hover {
  background-color: rgb(240, 240, 240);
}

.recentGameBoxLeft1:hover {
  background-color: rgba(240, 240, 240, 0.9);
}

.imgBox {
  position: absolute;
  width: 10rem;
  height: 4rem;
}

.modal1 {
  position: absolute;
  left: 0rem;
  top: 10.1rem;
  width: 20rem;
  height: 5rem;
  background-color: white;
}

.modal2 {
  position: absolute;
  left: 0rem;
  top: 5.1rem;
  width: 10rem;
  height: 6rem;
  background-color: white;
}


/* 字体风格 */
.textDate {
  position: absolute;
  top: -2.5rem;
  left: 2.7rem;
  color: var(--colors-text-dark-172239100, #172239);
  font-family: 'Britannic';
  font-size: 2.5rem;
  font-style: normal;
  font-weight: 300;
  line-height: normal;
}

.textStatus {
  position: absolute;
  color: var(--colors-text-dark-172239100, #0f41ac);
  font-family: '华文楷体';
  font-size: 1.5rem;
  top: 2.2rem;
  left: 11rem;
}

.textLeague {
  position: absolute;
  color: var(--colors-text-dark-172239100, black);
  font-family: '等线';
  font-size: 2rem;
  top: 5rem;
  left: 5rem;
  font-weight: 550;
}

.textTeamName {
  position: absolute;
  color: var(--colors-text-dark-172239100, black);
  font-family: '仿宋';
  font-size: 2rem;
  top: -2rem;
  font-weight: 800;
}

.textScore {
  font-family: Verdana;
  font-size: 1.5rem;
  color: var(--colors-text-dark-172239100, black);
  font-weight: 800;
}

.recentGameTypo {
  font-family: Verdana;
  font-size: 1.3rem;
  color: var(--colors-text-dark-172239100, black);
  font-weight: 600;
  position: absolute;
  top: 21rem;
}

.textMatchDate {
  font-family: 'Britannic';
  font-size: 1rem;
  color: var(--colors-text-dark-172239100, black);
  font-weight: 600;
  position: absolute;
  top: -0.7rem;
}

.textMatchScore {
  font-family: Georgia;
  font-size: 2rem;
  color: var(--colors-text-dark-172239100, black);
  font-weight: 600;
  position: absolute;
  top: -0.7rem;
}

.textMatchOpp {
  font-family: Verdana;
  font-size: 1.5rem;
  color: var(--colors-text-dark-172239100, black);
  font-weight: 600;
  position: absolute;
  top: -0rem;
}

.textLive {
  position: absolute;
  font-family: Verdana;
  font-size: 1rem;
  color: var(--colors-text-dark-172239100, black);
  font-weight: 600;
  top: 12rem;
  left: 14.7rem;
}

.picTeam {
  position: absolute;
  width: 13rem;
  height: 13rem;
  top: 0.5rem;
  object-fit: cover;
}

.intoLiveImg {
  position: absolute;
  top: 24rem;
  left: 36.2rem;
  height: 23rem;
  border: 2px solid var(--colors-light-eaeaea-100, #EAEAEA);
  border-radius: 2rem;
}

.intoLiveModal {
  position: absolute;
  top: 24rem;
  left: 36.2rem;
  height: 23.5rem;
  width: 33.4rem;
  border: 0px;
  border-radius: 2rem;
  background-color: rgba(255, 255, 255, 0.4);
  transition: background-color 0.8s ease;
}

.intoLiveModal:hover {
  background-color: rgba(240, 240, 240, 0.7);
}

.LiveLogo {
  position: absolute;
  left: 12.7rem;
  top: 6rem;
  height: 8rem;
}
</style>
