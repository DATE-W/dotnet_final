<script>
import { UserFilled } from '@element-plus/icons-vue';
import axios from 'axios';
export default {
  data() {
    return {
      avatar: 'https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png',
      defaultActive: 'AdminUsers', // 默认选中的菜单项
      signInNum: 0,
      newUsrNum: 0,
      newPostNum: 0,
    };
  },
  mounted() {
    this.getYesInfo();
  },
  methods: {
    handleMenuSelect(index) {
      // 处理左侧导航栏菜单点击事件
      this.defaultActive = index;
      this.$router.push(`/${index}`);
    },
    async getYesInfo() {
      console.log("start get Yesterday Info")
      let response
      try {
        response = await axios.get('/api/Post/getYestdayInfo');
      } catch (err) {
        console.error(err);
        if (err.response.data.result == 'fail') {
          ElMessage.error(err.response.data.msg)
        } else {
          ElMessage.error("未知错误")
        }
        return
      }
      if (response.data.ok == 'no') {
        ElMessage.error("获取昨日信息失败");
      } else if (response.data.ok == 'yes') {
        this.signInNum = response.data.value.signInNum;
        this.newUsrNum = response.data.value.newUsrNum;
        this.newPostNum = response.data.value.newPostNum;
      }
      return
    },
  },
};
</script>

<template>
  <div class="dashboard">
    <el-layout>
      <el-sider>
        <!--头像 + 昵称-->
        <el-container class="user-profile">
          <el-avatar :size="100" :src="avatar" /><br />
          <span class="username">Administrator</span><br />
        </el-container>
        <!-- 左侧导航栏 -->
        <el-menu :default-active="defaultActive" mode="vertical" @select="handleMenuSelect" background-color="white">
          <!-- <el-menu-item><span style="color: #8e97ab;"><el-icon><location /></el-icon>导航</span></el-menu-item> -->
          <el-menu-item index="AdminUsers"><span style="color: rgb(115, 115, 115);"><el-icon>
                <House />
              </el-icon>用户管理</span></el-menu-item>
          <el-menu-item index="AdminPosts"><span style="color: rgb(115, 115, 115);"><el-icon>
                <Notebook />
              </el-icon>论坛管理</span></el-menu-item>
          <el-menu-item index="AdminNews"><span style="color: rgb(115, 115, 115);"><el-icon>
                <Trophy />
              </el-icon>新闻管理</span></el-menu-item>
          <el-menu-item index="AdminAnnouncement"><span style="color: rgb(115, 115, 115);"><el-icon>
                <Watch />
              </el-icon>公告管理</span></el-menu-item>
          <el-menu-item index="AdminPastAnc"><span style="color: rgb(115, 115, 115);"><el-icon>
                <Watch />
              </el-icon>往期公告</span></el-menu-item>
        </el-menu>
        <el-container class="sub-box" style="background-color: rgb(246, 148, 148);margin-top:10px;">
          <span class="sub-text">今日签到人数：{{ signInNum }} 人</span>
        </el-container>
        <el-container class="sub-box" style="background-color: rgb(156, 233, 49);margin-top:20px;">
          <span class="sub-text">今日新增用户：{{ newUsrNum }} 个</span>
        </el-container>
        <el-container class="sub-box" style="background-color: rgb(148, 174, 246);margin-top:20px;">
          <span class="sub-text">今日新增帖子：{{ newPostNum }} 个</span>
        </el-container>
      </el-sider>
    </el-layout>
  </div>
</template>
  
  
<style scoped>
.dashboard {
  background-color: white;
  position: fixed;
  height: 90vh;
  margin-top: 2vh;
  border-radius: 15px 15px 0 0;
  width: 20vw;
}

.el-menu {
  border: 0 !important;
  justify-content: center;
  /* 水平居中 */
}

.user-profile {
  display: flex;
  flex-direction: column;
  align-items: center;
  /* 居中对齐 */
  justify-content: center;
  /* 居中对齐 */
  padding: 10px;
}

.username {
  color: rgb(88, 88, 88);
}

.sub-box {
  position: relative;
  border-radius: 10px;
  height: 7vh;
  width: 100%;
}

.sub-text {
  margin-top: 1.7vh;
  margin-left: 5vw;
  height: 1.5vh;
  color: white;
}
</style>