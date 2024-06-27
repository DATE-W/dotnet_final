<template>
  <el-container style="height: 100vh; display: flex; flex-direction: column;">
    <el-header style="padding: 0;">
      <my-nav></my-nav>
    </el-header>

    <el-container>
      <el-aside :style="{
        backgroundImage: 'url(' + backgroundImage + ')',
        backgroundSize: 'cover', backgroundRepeat: 'no-repeat',
      }">
        <!-- 左侧一列 -->
        <!-- 用户信息与邮箱 -->
        <el-card class="personal-card" style="opacity:0.8">
          <div v-if="isAccount">
            <!-- 用户名 -->
            <div class="userNameLayout">
              <div class="userNameTypo">{{ userName }}</div>
              <div class="homeTeamTypo">{{ homeTeam }}</div>
              <div class="homeTeamTypo">{{ userTitle }}</div>
            </div>

            <!-- 头像图片 -->
            <el-avatar :src="avatarUrl" class="avatar"></el-avatar>
            <!-- 关注、粉丝、点赞数 -->
            <el-card shadow="hover" class="childcard" style="margin-top: 6vh;" @click="showfollowing">
              <div class="childcardInfo">
                <div>关注</div>
                <div>
                  {{ followCnt }}&nbsp;
                  <el-icon>
                    <ArrowRightBold />
                  </el-icon>
                </div>
              </div>
            </el-card>
            <el-card shadow="hover" class="childcard" @click="showfollowed">
              <div class="childcardInfo">
                <div>粉丝</div>
                <div>
                  {{ befollowCnt }}&nbsp;
                  <el-icon>
                    <ArrowRightBold />
                  </el-icon>
                </div>
              </div>
            </el-card>
            <el-card shadow="none" class="childcard">
              <div class="childcardInfo">
                <div>点赞</div>
                <div>
                  {{ likeCnt }}&nbsp;
                </div>
              </div>
            </el-card>
            <button class="btn2 personal-btn"
              style="width: 180px; height: 50px; margin-top: 8vh; left: 3vw;position: absolute;" @click="showtabs = true">
              <div style="font-size: 20px;">个人动态</div>
            </button>
            <button class="btn2 personal-btn"
              style="width: 180px; height: 50px; margin-top: 16vh; left: 3vw;position: absolute;" @click="showedit">
              <div style="font-size: 20px;">资料编辑</div>
            </button>



            <!-- 退出登录 -->
            <div style="display: flex; justify-content: center;">
              <el-button class="logout-button" style="opacity: 1;" type="danger" @click="logout">登出</el-button>
            </div>
          </div>
          <div v-else>
            <!-- 点击登录 -->
            <div @click="performLogin" class="loginButton">
              点击登录
            </div>
          </div>

        </el-card>


      </el-aside>
      <el-main v-if="isAccount">
        <div v-if="showtabs">
          <el-tabs type="card" tab-position="top" class="maintabs">
            <el-tab-pane label="我的动态">
              <detail :backgroundImage="detailBackgroundimage" />
            </el-tab-pane>
            <el-tab-pane label="我的帖子">
              <post />
            </el-tab-pane>
            <el-tab-pane label="我的收藏">
              <favorite />
            </el-tab-pane>
            <el-tab-pane label="我的积分">
              <credits />
            </el-tab-pane>
            <el-tab-pane label="我的签到">
              <checkin />
            </el-tab-pane>
            <el-tab-pane label="消息通知">
              <notification />
            </el-tab-pane>
          </el-tabs>
        </div>
        <div v-else>
          <div v-if="showfollow">
            <following @follow-event="func_follow" />
          </div>
          <div v-else>
            <befollowed @follow-event="func_follow" />
          </div>
        </div>
      </el-main>
    </el-container>
  </el-container>
</template>



<script>
import MyNav from './nav.vue';
import pDetail from './personalDetail.vue';
import pFavorite from './personalFavorite.vue';
import pNotification from './personalNotification.vue';
import pCredits from './personalCredits.vue';
import pCheckin from './personalCheckin.vue';
import pPost from './personalPost.vue';

import pFollowing from './personalFollowing.vue';
import pBefollowed from './personalBefollowed.vue';

import axios from 'axios';
import { ElMessage } from 'element-plus';
export default {
  components: {
    'my-nav': MyNav,
    'detail': pDetail,
    'favorite': pFavorite,
    'notification': pNotification,
    'credits': pCredits,
    'checkin': pCheckin,
    'post': pPost,
    'following': pFollowing,
    'befollowed': pBefollowed,
  },
  data() {
    return {
      isAccount: false, // 为true表示有账号登录
      showtabs: true, // 是否展示tab标签页
      showfollow: true, // 是否展示关注，只有showtabs为true的时候该变量有意义
      avatarUrl: "", // 头像url
      userName: "", // 用户名
      homeTeam: "", //主队名
      followCnt: 0,       // 关注数
      befollowCnt: 0,     // 被关注数
      likeCnt: 0,         // 被点赞总数
      userTitle: '',           // 积分
      backgroundImage: '', //左侧竖
      detailBackgroundimage: '',//右侧横
    };
  },
  mounted() {
    this.JudgeAccount();
    this.getTheme();
  },
  methods: {
    showedit() {
      this.$router.push('/personalEdit');
    },
    logout() {
      this.$router.push('/');
      localStorage.removeItem('token');
    },
    performLogin() {
      // 在这里执行登录操作的逻辑
      this.$router.push('/signin');
    },
    showfollowing() {
      this.showtabs = false;
      this.showfollow = true;
    },
    showfollowed() {
      this.showtabs = false;
      this.showfollow = false;
    },
    func_follow(data) {
      // this.JudgeAccount();
      if (data == 'follow') {
        this.followCnt += 1;
      }
      else if (data == 'unfollow') {
        this.followCnt -= 1;
      }
    },
    async getUserTitle(mypoint) {
      if (mypoint >= 0 && mypoint <= 9) return '平平无奇';
      if (mypoint >= 10 && mypoint <= 49) return '普通用户';
      if (mypoint >= 50 && mypoint <= 99) return '一贴成名';
      if (mypoint >= 100 && mypoint <= 499) return '球场金童';
      if (mypoint >= 500 && mypoint <= 999) return '明日之星';
      if (mypoint >= 1000) return '名人堂';
    },
    async JudgeAccount() {
      const token = localStorage.getItem('token');
      if (token == null) {
        this.isAccount = false;
        console.log(this.isAccount);
        return;
      }
      let response
      try {
        const headers = {
          Authorization: `Bearer ${token}`,
        };
        response = await axios.post('/api/User/profile', {}, { headers })
      } catch (err) {
        console.log(err);
        if (err.response.data.result == 'fail') {
          ElMessage({
            message: err.response.data.msg,
            grouping: false,
            type: 'error',
          })
        } else {
          ElMessage({
            message: '未知错误',
            grouping: false,
            type: 'error',
          })
          return
        }
        return
      }
      console.log('judgeaccount');
      //有账号
      if (response.data.ok == 'yes') {
        this.isAccount = true;
        console.log(this.isAccount);
        this.avatarUrl = response.data.value.avatar;
        this.userName = response.data.value.username;
        this.homeTeam = response.data.value.uft;
        this.followCnt = response.data.value.follower_num;
        this.befollowCnt = response.data.value.follow_num;
        this.likeCnt = response.data.value.approval_num;
        this.userTitle = await this.getUserTitle(response.data.value.userpoint);
      }
      else {
        this.isAccount = false;
        console.log(this.isAccount)
      }
      return
    },
    async getTheme() {
      const token = localStorage.getItem('token');
      if (token == null) {
        this.isAccount = false;
        console.log(this.isAccount);
        return;
      }
      let response
      try {
        const headers = {
          Authorization: `Bearer ${token}`,
        };
        response = await axios.post('/api/User/gettheme', {}, { headers })
      } catch (err) {
        console.log(err);
        ElMessage({
          message: '未知错误',
          grouping: false,
          type: 'error',
        })
        return
      }
      console.log(response);
      this.backgroundImage = response.data.image2;
      this.detailBackgroundimage = response.data.image1;
      return
    },

  }
}
</script>

<style scoped>
.loginButton {
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  text-decoration: underline;
  transition: color 0.3s, text-decoration 0.3s;
}

.loginButton:hover {
  color: blue;
  /* 修改为你希望的 hover 颜色 */
  text-decoration: none;
}

.personal-card {
  padding: 0px;
  /* 调整卡片的内边距 */
  margin: 10px;
  /* 调整卡片的外边距 */
  border: 1px solid #EAEAEA;
  /* 添加卡片的灰色边框 */
  border-radius: 8px;
  /* 设置卡片的圆角 */

  height: 600px;
}

.childcard {
  margin-top: 10px;
  font-size: 20px;
}

.childcardInfo {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.el-main {
  flex: 1;
  padding: 0;
}

/* 修改 el-tabs 样式，设置高度为 100% */
.maintabs {
  border: 0;
  border-left: 1px solid #EAEAEA;
  height: 100%;
}

:deep(.el-tabs__content) {
  padding: 0px;
}

:deep(.el-tabs__header) {
  margin: 0;
}

.logout-button {
  position: absolute;
  font-size: 24px;
  width: 150px;
  height: 60px;
  bottom: 5vh;
  opacity: 0.8;
  border-radius: 100px;
}

/* 按钮样式 */
.btn2 {
  flex-shrink: 0;
  border-radius: 16px;
  border: 1px solid var(--colors-light-eaeaea-100, #EAEAEA);
  /* 正式版本用此颜色*/
  background-color: #ffffff;

}

/* 文字风格 */
.userNameLayout {
  position: absolute;
  display: flex;
  width: 12vw;
  left: 7vw;
  flex-direction: column;
  flex-shrink: 0;
}

.userNameTypo {
  color: var(--colors-text-dark-172239100, #172239);
  font-family: Verdana;
  font-size: 22px;
  font-style: normal;
  font-weight: 600;
  line-height: 24px;
}

.homeTeamTypo {
  color: grey;
  font-family: Verdana;
  font-size: 18px;
  font-style: normal;
  font-weight: 600;
  line-height: 24px;
}

/* 图片风格 */
.avatar {
  width: 50px;
  height: 50px;
  border-radius: 48px;
  margin-top: 0.7vh;
  margin-left: 1vw;
  object-fit: cover;
}

.personal-btn {
  /* 按钮基本样式 */
  border-radius: 16px;
  border: 1px solid var(--colors-light-eaeaea-100, #EAEAEA);
  background-color: #ffffff;
  position: relative;
  transition: background-color 0.3s, color 0.3s, border-color 0.3s, transform 0.3s;
}

.personal-btn div {
  font-size: 20px;
  position: relative;
  z-index: 1;
}

/* 按钮悬停样式 */
.personal-btn:hover {
  background-color: var(--colors-light-eaeaea-100, #EAEAEA);
  border-color: var(--colors-light-eaeaea-100, #EAEAEA);
  color: var(--colors-text-dark-172239100, #172239);
  cursor: pointer;
  transform: scale(1.02);
}
</style>