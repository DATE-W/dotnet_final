<!-- 消息通知 v2.0 -->
<template>
  <div class="overflow-container">
    <div class="notice-container">
      <div class="title-container">
        <el-icon class="message-icon">
          <Message />
        </el-icon>
        <span class="notice-title">通知中心</span>
        <el-button class="clear-button" type="text" @click="clearConfirm">清空通知</el-button>
      </div>
      <hr class="separator">
      <div class="message-container">
        <div>
          <span class="message-title">最近收到的通知</span>
          <el-dropdown class="message-menu">
            <span class="el-dropdown-link">
              {{ selectedCategory }}
              <el-icon class="el-icon--right">
                <arrow-down />
              </el-icon>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item @click="selectCategory('全部类别')">全部类别</el-dropdown-item>
                <el-dropdown-item @click="selectCategory('赞同&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;')">赞同</el-dropdown-item>
                <el-dropdown-item @click="selectCategory('评论&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;')">评论</el-dropdown-item>
                <el-dropdown-item @click="selectCategory('收藏&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;')">收藏</el-dropdown-item>
                <el-dropdown-item @click="selectCategory('站务通知')">站务通知</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
        <hr class="separator">
        <div v-for="(date, index) in filteredDates" :key="index">
          <div class="notice-date">
            <span>{{ date }}</span>
          </div>
          <div v-for="(notificationIndex, innerIndex) in getNotificationsByDate(date)" :key="innerIndex">
            <div class="notice-item">
              <el-icon v-if="isLikeContent(notice_contents[notificationIndex])" class="icon-likes">
                <Mouse />
              </el-icon>
              <el-icon v-else-if="isCommentContent(notice_contents[notificationIndex])" class="icon-comment">
                <ChatLineSquare />
              </el-icon>
              <el-icon v-else-if="isOfficialContent(notice_people[notificationIndex])" class="icon-official">
                <ChromeFilled />
              </el-icon>
              <el-icon v-else-if="isCollectContent(notice_contents[notificationIndex])" class="icon-star">
                <Star />
              </el-icon>
              <span class="notice-people">{{ notice_people[notificationIndex] === 'notice' ? '站务通知' :
                notice_people[notificationIndex] }}</span>
              <span>&nbsp;&nbsp;</span>
              <span class="notice-content">{{
                getContentByType(notice_contents[notificationIndex], notice_people[notificationIndex], notificationIndex)
              }}</span>
            </div>
          </div>
          <hr class="separator" v-if="index !== filteredDates.length - 1">
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ElMessage, ElMessageBox } from 'element-plus';
import axios from 'axios';
export default {
  data() {
    return {
      notice_dates: [],
      notice_people: [],
      notice_contents: [],
      selectedCategory: '全部类别'
    };
  },
  mounted() {
    this.GetNotice();
  },
  computed: {
    filteredDates() {
      const uniqueDates = [...new Set(this.notice_dates)];
      return uniqueDates.filter(date => {
        const notificationsForDate = this.getNotificationsByDate(date);
        return notificationsForDate.length > 0;
      });
    },
    filteredNotifications() {
      if (this.selectedCategory === '全部类别') {
        return this.notice_dates.map((_, index) => index);
      } else if (this.selectedCategory.includes('赞同')) {
        return this.notice_contents.reduce((result, content, index) => {
          if (this.isLikeContent(content)) {
            result.push(index);
          }
          return result;
        }, []);
      } else if (this.selectedCategory.includes('评论')) {
        return this.notice_contents.reduce((result, content, index) => {
          if (this.isCommentContent(content)) {
            result.push(index);
          }
          return result;
        }, []);
      } else if (this.selectedCategory.includes('收藏')) {
        return this.notice_contents.reduce((result, content, index) => {
          if (this.isCollectContent(content)) {
            result.push(index);
          }
          return result;
        }, []);
      }
      else if (this.selectedCategory.includes('站务通知')) {
        return this.notice_people.reduce((result, content, index) => {
          if (this.isOfficialContent(content)) {
            result.push(index);
          }
          return result;
        }, []);
      } else {
        return [];
      }
    },
  },
  methods: {
    getNotificationsByDate(date) {
      return this.filteredNotifications.filter(notificationIndex => {
        return this.notice_dates[notificationIndex] === date;
      });
    },
    selectCategory(category) {
      this.selectedCategory = category;
    },
    isLikeContent(content) {
      return content.includes('like');
    },
    isCommentContent(content) {
      return content.includes('comment');
    },
    isOfficialContent(content) {
      return content.includes('notice');
    },
    isCollectContent(content) {
      return content.includes('collect');
    },
    clearConfirm() {
      ElMessageBox.confirm('点击确认以清空所有通知记录', '提示', {
        confirmButtonText: '确认',
        cancelButtonText: '取消',
        type: 'warning',
      })
        .then(() => {
          this.clearNotice();
          this.$message({
            type: 'success',
            message: '通知已成功清空',
          });
        })
        .catch(() => {
          // 用户点击取消按钮时执行的操作，这里可以不用做任何事情
        });
    },
    clearNotice() {
      this.notice_dates = [];
      this.notice_people = [];
      this.notice_contents = [];
    },
    getContentByType(content, people, notificationIndex) {
      if (content === 'like') {
        return '赞同了您的帖子';
      } else if (content === 'comment') {
        return '发表了一条评论';
      } else if (content === 'collect') {
        return '收藏了您的帖子';
      } else if (people === 'notice') {
        return this.notice_contents[notificationIndex];
      }
      else {
        return '未知通知类型';
      }
    },
    StandardDate(datetime) {
      const date = new Date(datetime);
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');
      return `${year}-${month}-${day}`;
    },
    async GetNotice() {
      const token = localStorage.getItem('token');
      let response
      try {
        const headers = {
          Authorization: `Bearer ${token}`,
        };
        response = await axios.post('/api/User/Notice', {}, { headers });
        this.notice_dates = response.data.notice_dates.map(date => this.StandardDate(date));
        this.notice_people = response.data.notice_people;
        this.notice_contents = response.data.notice_contents;
        console.log('通知', response);
      } catch (err) {
        ElMessage({
          message: '服务器错误，获取通知失败',
          grouping: false,
          type: 'error',
        });
      }
    },
  },
};
</script>
  
<style scoped>
.overflow-container {
  overflow-y: auto;
  max-height: 625px;
}

.overflow-container::-webkit-scrollbar {
  width: 0;
}

.notice-container {
  align-items: center;
  font-weight: bold;
}

.title-container {
  align-items: center;
  display: flex;
}

.message-icon {
  font-size: 45px;
}

.notice-title {
  margin-left: 8px;
  font-size: 23px;
}

.clear-button {
  margin-left: 950px;
  margin-bottom: -10px;
}

.message-container {
  margin-top: 20px;
  align-items: center;
  font-weight: bold;
  width: 70%;
  margin: 0 auto;
}

.message-title {
  font-weight: bold;
  font-size: 18px;
}

.separator {
  width: 100%;
  border: 0.5px solid #e6e6e6;
  margin: 10px auto;
}

.message-menu {
  margin-top: 5px;
  margin-left: 580px;
}

.notice-date {
  color: #cccccc;
  font-size: 14px;
  margin-bottom: 20px;
}

.notice-item {
  margin-bottom: 10px;
}

.notice-people {
  font-size: 17px;
  font-weight: bold;
}

.notice-content {
  font-family: SimHei;
  font-size: 16px;
  font-weight: normal;
}

.icon-likes {
  font-size: 20px;
  background-color: #ffff80;
  border-radius: 50%;
  padding: 10px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  margin-right: 8px;
}

.icon-comment {
  font-size: 20px;
  background-color: #b3d9ff;
  border-radius: 50%;
  padding: 10px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  margin-right: 8px;
}

.icon-official {
  font-size: 20px;
  background-color: #99ffbb;
  border-radius: 50%;
  padding: 10px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  margin-right: 8px;
}

.icon-star {
  font-size: 20px;
  background-color: #82f977;
  border-radius: 50%;
  padding: 10px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  margin-right: 8px;
}
</style>