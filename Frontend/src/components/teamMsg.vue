<template>
  <my-nav></my-nav>
  <!-- 返回按钮 -->
  <el-button class="button" @click="goBack()">
    <el-icon><Back /></el-icon>
    <a>返回</a>
  </el-button>
  <!-- 上方数据 -->
  <el-card class="teamNameAndIcon">
    <img class="teamIconContainer" alt="This is team icon" :src="this.logo">
    <div class="teamNameContainer">
      {{ this.teamName }}
    </div>
    <div class="teamEnnameContainer">
      {{ this.enName }}
    </div>
  </el-card>
  <!-- 下方数据 -->
  <el-card class="teamInfo">
    <div class="firstBar">
      <div class="firstBlock">
        <b>成立：</b>{{ this.foundYear }}
      </div>
      <div class="secondBlock">
        <b>国家：</b>{{ this.country }}
      </div>
      <div class="thirdBlock">
        <b>联赛类别：</b>{{ this.leagueType }}
      </div>
      <div class="fourthBlock">
        <b>主教练：</b>{{ this.coach }}
      </div>
    </div>

    <div class="secondBar">
      <div class="firstBlock">
        <b>城市：</b>{{ this.city }}
      </div>
      <div class="secondBlock">
        <b>地址：</b>{{ this.address }}
      </div>

      <div class="thirdBlock">
        <b>主场：</b>{{ this.venueName }}
      </div>
      <div class="fourthBlock">
        <b>容纳：</b>{{ this.venueCapacity }}人
      </div>
    </div>

    <div class="thirdBar">
      <div class="firstBlock">
        <b>电话：</b>{{ this.tel }}
      </div>
      <div class="secondBlock">
        <b>邮箱：</b>{{ this.email }}
      </div>
    </div>

    <div class="fourthBar">
      <b>球队简介：</b>
      <div class="fourthBarContent">
        {{ this.teamName }}足球俱乐部（英文名：{{ this.enName }}），是{{ this.leagueType }}的一支足球俱乐部。
        {{ this.teamName }}成立于{{ this.foundYear }}年，坐落于美丽的{{ this.city }}。
        {{ this.teamName }}的主场球场为{{ this.venueName }}，能够容纳{{ this.venueCapacity }}人同时观赛。
      </div>
    </div>
  </el-card>

  <div class="titleContainer">
    <div class="titleBar"></div>
    <div class="title">
      <p style="transform: translateY(-25%); ">球员信息</p>
    </div>
  </div>
  <!-- 球员表格 -->
  <el-table :data="currentPlayerPage" style="width: 70vw; left: 16vw; top: 70vh;" @row-click="handleRowClick">
    <!-- 表格列配置 -->
    <el-table-column align="center" prop="playerNumber" label="号码" min-width="12%" />
    <el-table-column align="center" prop="playerPosition" label="位置" min-width="12%" />

    <el-table-column align="right" min-width="12%">
      <template #default="scope">
        <div>
          <img :src="scope.row.playerPhoto" alt="Player Photo" class="player-photo">
        </div>
      </template>
    </el-table-column>

    <el-table-column align="left" prop="playerName" label="姓名" min-width="16%" />
    <el-table-column align="center" prop="playerAppearance" label="出场" min-width="12%" />
    <el-table-column align="center" prop="playerShoot" label="射门" min-width="12%" />
    <el-table-column align="center" prop="playerGoal" label="进球" min-width="12%" />
    <el-table-column align="center" prop="playerNationality" label="国籍" min-width="12%" />

  </el-table>
  <!-- 分页功能 -->
  <div class="pagination">
    <el-pagination :current-page="currentPage" :page-size="pageSize" :total="totalTeamMembers"
      @current-change="handlePageChange">
    </el-pagination>
  </div>
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
    this.teamName = this.$route.query.teamName;
  },

  mounted() {
    this.getTeamMsg(this.teamName);
  },

  computed: {
    currentPlayerPage() {
      const start = (this.currentPage - 1) * this.pageSize;
      const end = start + this.pageSize - 1;
      return this.teamMember.slice(start, end);
    },
  },

  methods: {
    direct2detailedPlayerMsg(name) {
      this.$router.push({
        path: '/detailedPlayerMsg',
        query: {
          playerName: name
        }
      });
    },

    async getTeamMsg(teamName) {
      let response;
      this.teamName = teamName;
      try {
        response = await axios.post('/api/updateTeam/getTeamInfoByName', {
          teamName: teamName,
        });

        this.teamMember = [];

        this.city = response.data[0].city;
        this.foundYear = response.data[0].foundYear;
        this.enName = response.data[0].enName;
        this.logo = response.data[0].logo;
        this.country = response.data[0].country;
        this.venueName = response.data[0].venue_name;
        this.tel = response.data[0].telephone;
        this.address = response.data[0].address;
        this.venueCapacity = response.data[0].venue_capacity;
        this.email = response.data[0].email;
        this.coach = response.data[0].coach;
        this.leagueType = response.data[0].leagueType;

        this.teamMember = response.data[0].teamMember;
        this.totalTeamMembers = this.teamMember.length;

      } catch (err) {
        ElMessage({
          message: '获取球队信息失败',
          type: 'error',
        });
      }
    },

    handleRowClick(row, event, column) {
      this.direct2detailedPlayerMsg(row.playerName);
    },

    handlePageChange(newPage) {
      // 当用户切换页码时触发的方法
      this.currentPage = newPage;
    },

    goBack() {
      this.$router.back();
    },
  },

  data() {
    return {
      teamName: '',
      enName: '',
      logo: '',
      coach: '',
      foundYear: '',
      city: '',
      address: '',
      country: '',
      venueName: '',
      venueCapacity: 0,
      tel: '',
      email: '',
      leagueType: '',

      teamMember: [
      ],

      totalTeamMembers: 0,
      pageSize: 8,
      currentPage: 1,

    }

  }
}
</script>

<style scoped>
/* 顶部容器 */
.teamNameAndIcon {
  position: absolute;
  left: 15vw;
  width: 70vw;
  height: 20vh;
  flex-shrink: 0;
  background-image: linear-gradient(to right, white 20%, rgba(255, 255, 255, 0.6)), url('/src/assets/img/backgroundPic.png');
  background-size: cover;
  /* 可根据需要调整背景大小 */
  background-repeat: no-repeat;
  background-position: center;
  /* 可根据需要调整背景位置 */
}

.teamIconContainer {
  position: absolute;
  left: 3vw;
  top: 2.5vh;
  width: 8vw;
  height: 15vh;
  flex-shrink: 0;
}

.teamNameContainer {
  position: absolute;
  left: 13vw;
  top: 4vh;
  font-size: 2.5vw;
}

.teamEnnameContainer {
  position: absolute;
  left: 13vw;
  top: 11vh;
  font-size: 1.5vw;
}

.teamInfo {
  position: absolute;
  left: 15vw;
  top: 32vh;
  width: 70vw;
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
  left: 34vw;
}

.fourthBlock {
  position: absolute;
  left: 49vw;
}

.fourthBarContent {
  position: absolute;
  left: 5.4vw;
  top: 0vh;
  height: 13vh;
  width: 50vw;
}

.titleContainer {
  position: absolute;
  border: none;
  top: 70vh;
  left: 15vw;
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

.player-photo {
  width: 50px;
  height: 50px;
  object-fit: cover;
  border-radius: 20%;
}

.pagination {
  position: absolute;
  left: 35vw;
  top: 155vh;
  width: 50px;
  height: 50px;
}

.button{
  position: absolute;
  display: flex;
  justify-content: space-between;
  align-items: center;
  left: 10vw;
}
</style>