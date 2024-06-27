<!-- 2152190 李凌朗  2153398 王晗 -->
<!-- 样式大改 添加视频接口 完善搜索接口 -->
<template>
  <my-nav></my-nav>
  <el-container>
    <el-main class="main-container">
      <!-- 展示视频列表 -->
      <el-carousel :interval="4000" type="card" height="130px">
        <el-carousel-item v-for="(video, index) in PlayerVideos" :key="index">
          <img :src="video.cover" @click="goToVideo(video.url)" class="carousel-image" />
        </el-carousel-item>
      </el-carousel>
      <div class="up-container">
        <!-- 选择联赛栏 -->
        <div class="league-buttons">
          <button v-for="(league, index) in leagues" :key="index" @click="leagueChoice(index)"
            :class="{ active: SelectedLeague === index }">
            {{ league.name }}
          </button>
        </div>
        <!-- 搜索框 -->
        <div class="search-container">
          <el-icon class="search-icon">
            <Search />
          </el-icon>
          <el-input class="search-input" v-model="keyword" placeholder="请输入关键词"
            @keyup.enter.native="SearchForName(this.keyword, this.SelectedLeague)"></el-input>
          <el-button v-if="ShowReturnButton" icon="ArrowLeft" @click="ReturnToBegin"></el-button>
        </div>
      </div>
      <!-- 展示队伍 -->
      <el-container>
        <el-aside class="league-logo">
          <img :src="leagues[SelectedLeague].logo" alt="League Logo" class="logo-show" />
          <div class="league-name">{{ leagues[SelectedLeague].name }}</div>
          <div class="league-description">{{ leagues[SelectedLeague].description }}</div>
        </el-aside>
        <div class="ShowResults">
          <div class="result-container"
            v-for="(team, index) in NormalTeamsinLeague.slice((currentPage - 1) * pageSize, currentPage * pageSize)"
            :key="index">
            <div class="result-info" @click="goToTeam(team.teamName)">
              <img :src="team.teamLogo" alt="Team Logo" class="result-logo" />
              <div class="result-name">{{ team.teamName }}</div>
            </div>
          </div>
          <div class="result-container"
            v-for="(team, index) in SearchTeamsinLeague.slice((currentPage - 1) * pageSize, currentPage * pageSize)"
            :key="index">
            <div class="result-info" @click="goToTeam(team.teamName)">
              <img :src="team.teamLogo" alt="Team Logo" class="result-logo" />
              <div class="result-name">{{ team.teamName }}</div>
            </div>
          </div>
          <div class="result-container"
            v-for="(player, index) in PlayersinLeague.slice((currentPage - 1) * pageSize, currentPage * pageSize)"
            :key="index">
            <div class="result-info" @click="goToPlayer(player.searchedPlayerName)"
              v-if="player.searchedPlayerName !== 0">
              <img :src="player.searchedPlayerPhoto" alt="Team Logo" class="result-logo" />
              <div class="result-name">{{ player.searchedPlayerName }}</div>
            </div>
          </div>
        </div>
      </el-container>
      <!-- 分页栏 -->
      <div class="page-container">
        <el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
          layout="prev, pager, next, jumper" :total="totalTeams"></el-pagination>
      </div>
    </el-main>
    <el-aside width="350px">
      <!-- 展示射手榜 -->
      <div class="rank-title">
        射手榜
      </div>
      <div class="rank-scorers-title">
        <div class="rank-header">排名</div>
        <div class="rank-header">球员名</div>
        <div class="rank-header">进球数</div>
      </div>
      <div v-for="(topScorer, index) in topScorers" :key="index" class="rank-scorers">
        <div class="rank-cell">{{ index + 1 }}</div>
        <div class="rank-cell" @click="goToPlayer(topScorer.topScorerName)">{{ topScorer.topScorerName }}</div>
        <div class="rank-cell">{{ topScorer.goals }}</div>
      </div>
    </el-aside>
  </el-container>
</template>

<script>
import axios from 'axios';
import MyNav from './nav.vue';
import { ElMessage } from 'element-plus';

export default {
  data() {
    return {
      PlayerVideos: [],//存储视频及封面
      leagues: [
        { "name": "全部赛事", "logo": "/src/assets/img/football_logo.png", "description": "本网站展示中超及五大联赛的各个球队，右侧为所有球队，点击球队队徽可查看球队详情。" },
        { "name": "英超", "logo": "/src/assets/img/pmlogo.png", "description": "英国顶级足球联赛，有曼联、利物浦、切尔西等球队，以激烈比赛和全球球迷而闻名。" },
        { "name": "西甲", "logo": "/src/assets/img/lllogo.png", "description": "西班牙顶级足球联赛，皇马、巴萨等球队角逐，以技术流、激烈竞争著称。" },
        { "name": "意甲", "logo": "/src/assets/img/salogo.png", "description": "意大利顶级足球联赛，有尤文图斯、AC米兰等球队，防守严密，历史悠久。" },
        { "name": "德甲", "logo": "/src/assets/img/bllogo.png", "description": "德国顶级足球联赛，有拜仁慕尼黑、多特蒙德等球队，攻势足球，崇尚对抗。" },
        { "name": "法甲", "logo": "/src/assets/img/le1logo.png", "description": "法国顶级足球联赛，巴黎圣日耳曼、马赛等球队竞争，新锐天才与老将共舞。" },
        { "name": "中超", "logo": "/src/assets/img/cslogo.png", "description": "中国顶级足球联赛，广州恒大、上海上港等球队，崛起中展现实力。" },
      ],//存储联赛名、联赛logo及介绍
      NormalTeamsinLeague: [], // 存储正常结果中的球队
      SearchTeamsinLeague: [], // 存储搜索结果中的球队
      PlayersinLeague: [],//存储当前联赛包含的球员
      topScorers: [],//存储射手榜
      pageSize: 20,//每页最多20项
      currentPage: 1,//初始为第一页
      totalTeams: 0,//存储队伍总数
      SelectedLeague: 0,//用数字代替选择的联赛名
      keyword: '',//保存搜索关键词
      ShowReturnButton: false,//搜索返回键 初始不展示
    };
  },
  methods: {
    goToVideo(video_url) {
      window.open(video_url);
    },
    goToTeam(TeamName) {
      this.$router.push({
        path: '/teamMsg',
        query: { teamName: TeamName }
      })
    },
    goToPlayer(PlayerName) {
      this.$router.push({
        path: '/detailedPlayerMsg',
        query: { playerName: PlayerName }
      });
    },
    handlePageChange(currentPage) {
      this.currentPage = currentPage;
    },
    leagueChoice(index) {
      this.SelectedLeague = index;
      this.getTeam(index);
    },
    SearchForName(keyword, SelectedLeague) {
      this.ShowReturnButton = true;
      this.SearchForTeam(keyword, SelectedLeague);
      this.SearchForPlayer(keyword, SelectedLeague);
    },
    ReturnToBegin() {
      this.ShowReturnButton = false;
      this.getTeam(this.SelectedLeague);
    },
    async getPlayerVideos() {
      let response
      try {
        response = await axios.get('/api/Highlight/getHighlights');
        if (response.data.ok === 'yes') {
          console.log('entry in');
          this.PlayerVideos = response.data.value.map(item => {
            return {
              cover: item.photo,
              url: item.videoUrl,
            };
          });
        }
        console.log('视频列表为', this.PlayerVideos);
      } catch (err) {
        ElMessage({
          message: '服务器错误，获取球员剪辑失败',
          type: 'error',
        });
      }
    },
    async getTopScorer() {
      let response
      try {
        response = await axios.get('/api/updateTeam/getTopScorers');
        this.topScorers = response.data;
      } catch (err) {
        ElMessage({
          message: '服务器错误，获取射手榜失败',
          type: 'error',
        });
      }
    },
    async getTeam(SelectedLeague) {
      this.PlayersinLeague = [];
      this.SearchTeamsinLeague = [];
      let response
      try {
        response = await axios.post('/api/updateTeam/searchTeamInGameType', {
          gameType: SelectedLeague,
        });
        this.NormalTeamsinLeague = [];
        this.NormalTeamsinLeague = response.data;
        this.totalTeams = this.NormalTeamsinLeague.length;
      } catch (err) {
        ElMessage({
          message: '服务器错误，获取球队信息失败',
          type: 'error',
        });
      }
    },
    async SearchForTeam(keyword, SelectedLeague) {
      let response
      try {
        response = await axios.post('/api/updateTeam/searchForTeam', {
          key: keyword,
          gameType: SelectedLeague,
        });
        this.NormalTeamsinLeague = [];
        this.SearchTeamsinLeague = [];
        for (let i = 0; i < response.data.length; i++) {
          const newTeam = { teamLogo: response.data[i].searchedTeamLogo, teamName: response.data[i].searchedTeamName }; // 创建新对象并设置属性
          this.SearchTeamsinLeague.push(newTeam); // 将对象添加到数组中
        }
      } catch (err) {
        ElMessage({
          message: '服务器错误，搜索球队失败',
          type: 'error',
        });
      }
    },
    async SearchForPlayer(keyword, SelectedLeague) {
      let response;
      try {
        response = await axios.post('/api/updateTeam/searchForPlayer', {
          key: keyword,
          gameType: SelectedLeague,
        });
        // 检查SearchTeamsinLeague的长度并设置初始占位符的数量
        let placeholdersCount = 0;
        if (this.SearchTeamsinLeague.length > 0 && this.SearchTeamsinLeague.length <= 20) {
          placeholdersCount = 20;
        } else if (this.SearchTeamsinLeague.length > 20 && this.SearchTeamsinLeague.length <= 40) {
          placeholdersCount = 40;
        }
        // 使用占位符填充PlayersinLeague
        this.PlayersinLeague = Array(placeholdersCount).fill({ searchedPlayerPhoto: 0, searchedPlayerName: 0 });
        // 将response.data附加到PlayersinLeague
        this.PlayersinLeague = this.PlayersinLeague.concat(response.data);

        this.totalTeams = this.PlayersinLeague.filter(player => player.searchedPlayerName !== 0).length +
          (this.SearchTeamsinLeague.length > 0 && this.SearchTeamsinLeague.length <= 20 ? 20 : 0) +
          (this.SearchTeamsinLeague.length > 20 && this.SearchTeamsinLeague.length <= 40 ? 40 : 0);
      } catch (err) {
        ElMessage({
          message: '服务器错误，搜索球员失败',
          type: 'error',
        });
      }
    },
  },
  components: {
    MyNav,
  },
  mounted() {
    this.getTeam(0);
    this.getTopScorer();
    this.getPlayerVideos();
  },
}
</script>

<style scoped>
.carousel-image {
  width: 80%;
  max-height: 130px;
  display: block;
  margin: auto;
}

.up-container {
  /* 联赛选择按钮和搜索框所在的container */
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: -20px;
}

/* 以下均为联赛选择按钮 */
.league-buttons {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-top: 20px;
}

.league-buttons button {
  background-color: white;
  color: black;
  border: 1px solid #ccc;
  font-weight: bold;
  font-size: 15px;
  border-radius: 20px;
  padding: 10px 24px;
  cursor: pointer;
  transition: background-color 0.3s, color 0.3s;
}

.league-buttons button:hover {
  background-color: #f0f0f0;
}

.league-buttons button.active {
  background-color: #3ba7ea;
  color: white;
  border-color: #3ba7ea;
}

/* 搜索框样式 */
.search-container {
  margin-top: 15px;
  display: flex;
  align-items: center;
}

.search-icon {
  font-size: 24px;
}

.search-input {
  width: 100%;
}

.league-logo {
  /* 展示内容的左侧介绍 */
  margin-top: 70px;
  margin-left: 20px;
  margin-right: 10px;
  width: 20%;
  float: left;
}

.logo-show {
  /* 展示对应的联赛logo */
  width: 75px;
  height: 75px;
  border-radius: 50%;
}

.league-name {
  /* 展示对应联赛的名字 */
  font-weight: bold;
  margin-top: 10px;
  font-size: 30px;
}

.league-description {
  /* 展示对应联赛的简介 */
  font-weight: lighter;
  font-size: 15px;
  margin-top: 20px;
}

.ShowResults {
  margin-top: 50px;
  width: 80%;
  float: left;
  display: flex;
  flex-wrap: wrap;
  justify-content: flex-start;
}

/* 以下为每页展示的队伍/球员logo和name */
.result-container {
  width: 20%;
  box-sizing: border-box;
  padding: 10px;
  display: flex;
  flex-direction: column;
}

.result-info {
  display: flex;
  align-items: center;
  margin-top: 20px;
}

.result-logo {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  margin-right: 10px;
}

.result-name {
  font-size: 15px;
}

.grey-separator {
  /* 分割线 */
  display: inline-block;
  width: 100%;
  max-width: 100%;
  height: 1px;
  background-color: #ccc;
}

.rank-title {
  /* 射手榜标题 */
  background-color: #fffafa;
  font-weight: bold;
  font-size: 25px;
  text-align: center;
}

.rank-scorers-title {
  /* 射手榜整体榜头 */
  background-color: #fffafa;
  padding-top: 15px;
  display: grid;
  grid-template-columns: 1fr 2fr 1fr;
  align-items: center;
}

.rank-scorers {
  /* 射手榜 */
  background-color: #fffafa;
  padding-top: 19.4px;
  padding-bottom: 20px;
  display: grid;
  grid-template-columns: 1fr 2fr 1fr;
  align-items: center;
}

.rank-header {
  /* 射手榜榜头 */
  background-color: #28f06b;
  color: white;
  text-align: center;
  padding: 10px;
  font-weight: bold;
}

.rank-cell {
  /* 射手榜内容 */
  text-align: center;
  color: black;
}

.page-container {
  /* 分页栏 */
  position: absolute;
  bottom: 0px;
  left: 37%;
  transform: translateX(-50%);
}
</style>