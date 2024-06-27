<template>
  <el-header>
    <div class="nav-container">
      <div class="nav-left">
        <div class="nav-logo">
          <el-image src="/src/assets/img/tjlogo.png" style="width: 50px;height: 50px;"></el-image>
        </div>
        <el-menu mode="horizontal" :ellipsis="false">
          <el-menu-item :index="1" @click="redirectToMain" :class="{ 'blue-text': menutextcolor === 2 }">首页</el-menu-item>
          <el-menu-item :index="2" @click="redirectToNews" :class="{ 'blue-text': menutextcolor === 3 }">新闻</el-menu-item>
          <el-menu-item :index="3" @click="redirectToForum"
            :class="{ 'blue-text': menutextcolor === 4 }">论坛</el-menu-item>
          <el-menu-item :index="4" @click="redirectToGames"
            :class="{ 'blue-text': menutextcolor === 5 }">赛事</el-menu-item>
          <el-menu-item :index="5" @click="redirectToPlayers"
            :class="{ 'blue-text': menutextcolor === 6 }">球员信息</el-menu-item>
        </el-menu>
      </div>
      <div class="nav-right">
        <el-dropdown trigger="hover">
          <el-avatar :src=this.avatarurl alt="Avatar" class="avatar"></el-avatar>
          <template #dropdown>
            <el-dropdown-menu v-slot: dropdown>
              <el-dropdown-item @click="redirectToLogin(0)" v-if="!this.islog">用户登录</el-dropdown-item>
              <el-dropdown-item @click="redirectToLogin(1)" v-if="!this.islog">管理员登录</el-dropdown-item>
              <el-dropdown-item @click="redirectToRegister" v-if="!this.islog">注册</el-dropdown-item>
              <el-dropdown-item @click="redirectToPersonal" v-if="this.islog">个人中心</el-dropdown-item>
              <el-dropdown-item @click="logout" v-if="this.islog">登出</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
        <span class="user-nickname">{{ this.userName }}</span>
      </div>
    </div>
  </el-header>
</template>
  

<script>
import axios from 'axios';
import { ElMessage } from 'element-plus';
export default {
  data() {
    return {
      islog: false,
      avatarurl: "https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png",
      userName: '',
    }
  },
  mounted() {
    this.JudgeAccount();
  },
  computed: {
    menutextcolor() {
      if (this.$route.path === '/') {
        return 2;
      } else if (this.$route.path === '/News') {
        return 3;
      } else if (this.$route.path === '/forum') {
        return 4;
      } else if (this.$route.path === '/Games') {
        return 5;
      } else if (this.$route.path === '/Players') {
        return 6;
      }
    },
  },
  methods: {
    async JudgeAccount() {
      const token = localStorage.getItem('token');
      if (token == null) {
        this.islog = false;
        console.log(this.islog);
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
      console.log(response);
      //有账号
      if (response.data.ok == 'yes') {
        this.islog = true;
        this.userName = response.data.value.username;
        this.avatarurl = response.data.value.avatar;
      }
      else {
        this.islog = false;
      }
    },
    redirectToLogin(isAdmin) {
      // 跳转到登录页面的逻辑
      this.$router.push({
        path: '/signin',
        query: {
          isAdmin: isAdmin
        }
      });
    },
    redirectToRegister() {
      // 跳转到注册页面的逻辑
      this.$router.push('/signup');
    },
    redirectToForum() {
      //跳转到论坛页面的逻辑
      this.$router.push('/forum')
    },
    redirectToMain() {
      //跳转到首页页面的逻辑
      this.$router.push('/')
    },
    redirectToPersonal() {
      //跳转到个人中心页面的逻辑
      this.$router.push('/personal')
    },
    redirectToGames() {
      //跳转到赛事页面的逻辑
      this.$router.push('/Games')
    },
    redirectToNews() {
      //跳转到新闻页面的逻辑
      this.$router.push('/News')
    },
    redirectToPlayers() {
      //跳转到新闻页面的逻辑
      this.$router.push('/Players')
    },
    logout() {
      localStorage.removeItem('token');
      this.$router.push('/');
      setTimeout(() => {
        window.location.reload(); // 刷新当前页面
      }, 100); // 2000毫秒后刷新，你可以根据需要调整延迟时间
    }
  }
};
</script>
<style scoped>
.nav-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 60px;
  padding: 0 20px;
}

.nav-left {
  display: flex;
  align-items: center;
}

.nav-logo {
  font-size: 20px;
  font-weight: bold;
  margin-right: 20px;
}

.nav-right {
  display: flex;
  align-items: center;
}


.avatar {
  width: 50px;
  height: 50px;
  border-radius: 48px;
  margin-right: 10px;
  object-fit: cover;
}

.user-nickname {
  margin-right: 20px;
}

.blue-text {
  color: #3ba7ea;
}
</style>
  