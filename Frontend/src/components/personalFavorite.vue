<template>
  <div class="my-posts-container">
    <div class="posts-section">
      <h2>我的收藏</h2>
      <div class="overflow-container">
        <div class="post-list">
          <div class="center-wrapper" v-if="post_id.length === 0">
            <div class="no-posts">
              <div class="icon-wrapper">
                <el-icon size="120">
                  <DocumentRemove />
                </el-icon>
              </div>
              <div class="text-wrapper">
                <p class="no-posts-text">您还没有收藏过帖子</p>
              </div>
            </div>
          </div>
          <div v-for="(post, index) in post_id" :key="post.id" class="post-item" @click="goToDetail(post_id[index])">
            <div class="post-title">{{ post_title[index] }}</div>
            <div class="post-content">{{ post_content[index] }}</div>
            <div class="info-group">
              <span class="like-container">
                <el-icon size="medium">
                  <Pointer />
                </el-icon>
                <span class="like-number">{{ post_likes[index] }}</span>
              </span>
              <span class="space-between"></span>
              <span class="star-container">
                <el-icon size="medium">
                  <Star />
                </el-icon>
                <span class="star-number">{{ post_stars[index] }}</span>
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="points-section">
      <div class="points-box">
        <h3 style="color: aliceblue;">我的积分</h3>
        <div class="points">{{ myPoints }}</div>
        <div class="user-title" style="margin-top: 15px;color:rgb(255, 255, 255) ;font-weight: bold;">{{
          getUserTitle(myPoints) }}</div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import { ElIcon, ElMessage } from 'element-plus';

export default {
  data() {
    return {
      myPoints: 0,
      currentPage: 1,
      pageSize: 4,  //每页4项
      totalPosts: 0,
      showPage: false, //初始为false 向后端请求完数据后变为true 更换tag页面暂时变为false
      post_id: [],  //存储返回的帖子id
      post_title: [],  //存储返回的帖子标题
      post_content: [],
      post_likes: [],
      post_stars: [],
      currentTag: 'ALL',  //向后端传递当前页面的帖子类型 初始为全部 不受tag影响
    };
  },
  mounted() {
    this.getCollectPosts();
    this.getPoint();
  },
  methods: {
    goToDetail(postId) {
      this.$router.push({
        path: '/detail',
        query: { clickedPostID: postId }
      });
    },
    handleMouseOver(index) {
      this.postList[index].hovered = true;
    },
    handleMouseOut(index) {
      this.postList[index].hovered = false;
    },
    async getCollectPosts() {
      const token = localStorage.getItem('token');
      let response
      try {
        response = await axios({
          method: 'GET',
          url: '/api/User/GetMyCollectPost',
          headers: {
            Authorization: `Bearer ${token}`,
          },
        })

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
      if (Array.isArray(response.data)) {
        response.data.forEach((postInfo) => {
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
    async getPoint() {
      const token = localStorage.getItem('token');
      let response
      try {
        const headers = {
          Authorization: `Bearer ${token}`,
        };
        response = await axios.post('/api/User/userPoint', {}, { headers });
        //在这里获取到用户的积分数和积分明细
        // this.myPoints = response.data.myPoints;
        // this.usertitle = this.getUserTitle(this.myPoints);
        this.myPoints = response.data.userpoint;
      } catch (err) {
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
        }
        return
      }
      return
    },
    getUserTitle(myPoints) {
      if (myPoints >= 0 && myPoints <= 9) return '平平无奇';
      if (myPoints >= 10 && myPoints <= 49) return '普通用户';
      if (myPoints >= 50 && myPoints <= 99) return '一贴成名';
      if (myPoints >= 100 && myPoints <= 499) return '球场金童';
      if (myPoints >= 500 && myPoints <= 999) return '明日之星';
      if (myPoints >= 1000) return '名人堂';
    },
  },
};
</script>

  
<style scoped>
.overflow-container {
  overflow-y: auto;
  max-height: 460px;
}

.overflow-container::-webkit-scrollbar {
  width: 0;
}

.my-posts-container {
  display: flex;
  width: 100%;
  height: 82vh;
  background: #d7ecffeb;
  border-radius: 20px;
}

.posts-section {
  flex: 7;
  padding: 20px;
}

.post-list {
  margin-top: 50px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 20px;
  max-height: 500px;
  align-items: center;
  text-align: center;
}

.center-wrapper {
  display: inline-block;
}

.no-posts {
  font-size: 20px;
  color: #999;
  display: inline-block;
}

.icon-wrapper {
  margin-bottom: 10px;
}

.no-posts-text {
  margin-top: 10px;
}

.post-item {
  width: 80%;
  border: 1px solid #ccc;
  padding: 10px;
  border-radius: 8px;
  transition: transform 0.3s, box-shadow 0.3s, background-color 0.3s;
  position: relative;
  cursor: pointer;
}

.post-item:hover {
  transform: scale(1.05);
  /* 缩放效果，可以根据需要进行调整 */
  box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);
  /* 添加阴影效果 */
}

.post-item:hover .post-content {
  color: #3498db;
  /* 浅蓝色 */
}

.post-title {
  font-size: 18px;
  font-weight: bold;
}

.post-content {
  margin-top: 5px;
}

.post-item:nth-child(odd) {
  background: #f8f8f8;
}

.post-item:nth-child(even) {
  background: #ffffff;
}

.info-group {
  display: flex;
  align-items: center;
  margin-top: 10px;
}

.space-between {
  width: 20px;
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

/* 积分 */

.points-section {
  flex: 3;
  display: flex;
  justify-content: center;
  align-items: center;
  /* background-color: #f0f0f0; */
  height: 200px;
}

.points-box {
  text-align: center;
  padding: 20px;
  width: 150px;
  height: 150px;
  border-radius: 50%;
  background: #0058fc6f;
}

.points {
  font-size: 36px;
  font-weight: bold;
  color: white;
  margin-top: 10px;
}
</style>
  