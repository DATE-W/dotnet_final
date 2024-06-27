<template>
  <my-nav></my-nav>
  <!-- 返回按钮 -->
  <el-button class="button" @click="goBack()">
    <el-icon>
      <Back />
    </el-icon>
    <a>返回</a>
  </el-button>
  <!-- 上方数据 -->
  <el-card class="playerNameAndIcon">
    <!-- 球员图片 -->
    <img class="playerIconContainer" alt="This is player icon" :src="this.playerPhoto">
    <div class="playerNameContainer">
      {{ this.playerName }}
    </div>
    <div class="playerEnnameContainer">
      {{ this.enName }}
    </div>
  </el-card>
  <!-- 下方数据 -->
  <el-card class="playerInfo">
    <div class="firstBar">
      <div class="firstBlock">
        <b>国籍：</b>{{ this.nationality }}
      </div>
      <div class="secondBlock">
        <b>年龄：</b>{{ this.age }}
      </div>
      <div class="thirdBlock">
        <b>身高：</b>{{ this.height }}
      </div>
    </div>

    <div class="secondBar">
      <div class="firstBlock">
        <b>号码：</b>{{ this.number }}
      </div>
      <div class="secondBlock">
        <b>位置：</b>{{ this.position }}
      </div>

      <div class="thirdBlock">
        <b>俱乐部：</b>{{ this.club }}
      </div>
    </div>

    <div class="thirdBar">
      <div class="firstBlock">
        <b>惯用脚：</b>{{ this.dominantFoot }}
      </div>
    </div>

    <div class="fourthBar">
      <b>球员简介：</b>
      <div class="fourthBarContent">
        {{ this.playerName }}，（英文名：{{ this.enName }}），现年{{ this.age }}，
        是一名{{ this.nationality }}的足球运动员。他身高{{ this.height }}，现效力于{{ this.club }}足球俱乐部。
        他的惯用脚是{{ this.dominantFoot }}，在球队中担任{{ this.position }}的位置。他的球衣号码为{{ this.number }}。
      </div>
    </div>

  </el-card>

  <div class="titleContainer">
    <div class="titleBar"></div>
    <div class="title">
      <p style="transform: translateY(-25%); ">赛季数据</p>
    </div>
  </div>
  <!-- 球员数据 -->
  <el-table :data="eventData" style="width: 60vw; left: 10vw; top: 70vh;">
    <el-table-column align="center" prop="seasonName" label="赛季" width="158" />
    <el-table-column align="center" prop="appearance" label="上场" width="105" />
    <el-table-column align="center" prop="pass" label="过人" width="105" />
    <el-table-column align="center" prop="shoot" label="射门" width="105" />
    <el-table-column align="center" prop="goal" label="进球" width="105" />
    <el-table-column align="center" prop="assist" label="助攻" width="105" />
    <el-table-column align="center" prop="yellow" label="黄牌" width="105" />
    <el-table-column align="center" prop="red" label="红牌" width="105" />
  </el-table>

  <div class="titleContainer2">
    <div class="titleBar"></div>
    <div class="title">
      <p style="transform: translateY(-25%); ">相关球员</p>
    </div>
  </div>
  <!-- 相关球员 -->
  <el-table :data="relatedPlayers" style="width: 20vw; left: 74vw; top: -15vh;" @row-click="handleRowClick">

    <el-table-column align="right">
      <template #default="scope">
        <div>
          <img :src="scope.row.playerPhoto" alt="Player Photo" class="player-photo">
        </div>
      </template>
    </el-table-column>

    <el-table-column align="left" prop="playerName" width="150" />
    <el-table-column align="center" prop="type" width="60" />
  </el-table>
</template>

<script>
import { ref } from 'vue';
import MyNav from './nav.vue';
import { ElMessage } from 'element-plus';
import axios from 'axios';

export default {
  components: {
    'my-nav': MyNav
  },

  created() {
    this.playerName = this.$route.query.playerName;
  },

  mounted() {
    this.getPlayerMsg(this.playerName);
  },

  methods: {
    async getPlayerMsg(playerName) {
      this.playerName = playerName;
      let response;
      try {
        response = await axios.post('/api/updateTeam/getPlayerDetail', {
          playerName: playerName,
        });

        console.log(response);

        this.relatedPlayers = [];
        this.eventData = [];

        this.enName = response.data.enName;
        this.playerPhoto = response.data.photo;
        this.club = response.data.club;
        this.position = response.data.position;
        this.number = response.data.number;
        this.nationality = response.data.nationality;
        this.age = response.data.age;
        this.height = response.data.height;
        this.dominantFoot = response.data.dominantFoot;
        this.shoot = response.data.shoot;
        this.pass = response.data.pass;

        this.relatedPlayers = response.data.relatedPlayer;
        this.eventData = response.data.eventData;

      } catch (err) {
        ElMessage({
          message: '获取球员信息失败',
          type: 'error',
        });
      }

    },

    direct2detailedPlayerMsg(topScorerName) {
      this.$router.push({
        path: '/detailedPlayerMsg',
        query: {
          playerName: topScorerName
        }
      });
      this.getPlayerMsg(topScorerName);
    },

    handleRowClick(row, event, column) {
      this.direct2detailedPlayerMsg(row.playerName);
    },

    goBack() {
      this.$router.back();
    },
  },

  data() {
    return {
      playerName: '',
      enName: '',
      playerPhoto: '',
      club: '',
      position: '',
      number: '',
      nationality: '',
      age: '',
      height: '',
      dominantFoot: '',
      shoot: '',
      pass: '',

      eventData: ref([
      ]),

      relatedPlayers: ref([
      ]),

    }
  }

}
</script>

<style scoped>
.playerNameAndIcon {
  position: absolute;
  left: 10vw;
  width: 60vw;
  height: 20vh;
  flex-shrink: 0;
  background-image: linear-gradient(to right, white 20%, rgba(255, 255, 255, 0.6)), url('/src/assets/img/backgroundPic.png');
  background-size: cover;
  /* 可根据需要调整背景大小 */
  background-repeat: no-repeat;
  background-position: center;
  /* 可根据需要调整背景位置 */
}

.playerIconContainer {
  position: absolute;
  left: 3vw;
  top: 2.5vh;
  width: 8vw;
  height: 15vh;
  flex-shrink: 0;
}

.playerNameContainer {
  position: absolute;
  left: 14vw;
  top: 4vh;
  font-size: 2.5vw;
}

.playerEnnameContainer {
  position: absolute;
  left: 14vw;
  top: 11vh;
  font-size: 1.5vw;
}

.playerInfo {
  position: absolute;
  left: 10vw;
  top: 32vh;
  width: 60vw;
  height: 35vh;
  flex-shrink: 0;
}

/* 信息第一行 */
.firstBar {
  position: absolute;
  left: 4vw;
  top: 5vh;
  height: 4vh;
  width: 60vw;
}

/* 信息第二行 */
.secondBar {
  position: absolute;
  left: 4vw;
  top: 11vh;
  height: 4vh;
  width: 60vw;
}

/* 信息第三行 */
.thirdBar {
  position: absolute;
  left: 4vw;
  top: 17vh;
  height: 4vh;
  width: 60vw;
}

.fourthBar {
  position: absolute;
  left: 5vw;
  top: 23vh;
  height: 13vh;
  width: 62vw;
}

/* 横向第一条 */
.firstBlock {
  position: absolute;
  left: 1vw;
}

/* 横向第二条 */
.secondBlock {
  position: absolute;
  left: 16vw;
}

/* 横向第三条 */
.thirdBlock {
  position: absolute;
  left: 30vw;
}

.fourthBarContent {
  position: absolute;
  left: 5.4vw;
  top: 0vh;
  height: 13vh;
  width: 44vw;
}

.titleContainer {
  position: absolute;
  border: none;
  top: 70vh;
  left: 10vw;
  width: 7vw;
  height: 6vh;
  background: rgb(240, 240, 240);
}

.titleBar {
  position: absolute;
  top: 1.5vh;
  width: 0.5vw;
  height: 3vh;
  background: aqua;
}

.title {
  position: absolute;
  left: 1.5vw;
  width: 10vw;
  height: 6vh;
}

.titleContainer2 {
  position: absolute;
  border: none;
  top: 8vh;
  left: 75vw;
  width: 7vw;
  height: 6vh;
  background: rgb(240, 240, 240);
}

.player-photo {
  width: 50px;
  height: 50px;
  object-fit: cover;
  border-radius: 20%;
}

.button {
  position: absolute;
  display: flex;
  justify-content: space-between;
  align-items: center;
  left: 5vw;
}
</style>