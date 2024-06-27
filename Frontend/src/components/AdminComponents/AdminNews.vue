<script>
import dashboard from './AdminDashBoard.vue';
import headerview from './AdminNav.vue';
import axios from 'axios';
import { ElMessage } from 'element-plus';

export default {
    components:{
        dashboard:dashboard,
        headerview:headerview,
    },
    data() {
    return {
      news_id:[],
      news_tag1:[],
      news_tag2:[],
      news_title:[],
      news_summary:[],
      news_time:[],
      currentPage: 1,
      currentIndex:1,
      pageSize: 6,
      startIndex:0,
    };
  },
  mounted() {
    this.getAllNews();
  },
  computed: {
    displayedNewsIds() {
      const startIndex = (this.currentPage - 1) * this.pageSize;
      const endIndex = startIndex + this.pageSize;
      return this.news_id.slice(startIndex, endIndex);
    },
    totalPages() {
      return Math.ceil(this.news_id.length / this.pageSize);
    },
  },
  methods: {
    async deleteNews(newsId) {
      try {
        // 构建请求体，将新闻ID作为JSON数据发送
        const requestData = { id: newsId };
        
        // 发送删除请求，将 JSON 数据放在请求体中
        const response = await axios.delete('/api/Admin/DeleteNews', { data: requestData });

        // 在成功的情况下执行删除操作
        if (response.status === 200) {
          // 更新新闻列表，排除已删除的新闻
          ElMessage.success("成功删除该条新闻");
          // 等待一段时间后刷新页面
          setTimeout(() => {
            location.reload();
          }, 900); // 延迟时间，单位为毫秒
        }
      } catch (error) {
        console.error("删除新闻时出现错误：", error);
        return
      }
      return
    },
    //从后端获取新闻数据
    async getAllNews() {
      let response
          try {
            response = await axios({
                method: 'GET',
                url: '/api/Admin/GetAllNews',
              })
          } 
        catch (err) {
            ElMessage({
                message: '获取最近新闻信息失败',
                grouping: false,
                type: 'error',
            });
            return
        }
        
        if ( Array.isArray(response.data)) {
          response.data.forEach((postInfo) => {
              this.news_id.push(postInfo.news_id);
              this.news_title.push(postInfo.title);
              this.news_time.push(postInfo.publishDateTime);
              this.news_summary.push(postInfo.summary);
              this.news_tag1.push(postInfo.matchTag);
              this.news_tag2.push(postInfo.propertyTag);
          });
        }
        else {
            ElMessage({
                message: '后端返回的新闻数据格式错误',
                grouping: false,
                type: 'error',
            });
        }
        return
    },
    handlePageChange(newPage) {
      this.currentPage = newPage;
    },
    truncateText(text, maxLength) {
      if (text.length > maxLength) {
        return text.slice(0, maxLength) + '...';
      }
      return text;
    },
  },
}
</script>

<template>
     <div id="building">
        <el-container class="rooter-box">
        <el-header class="hide-header">
            <headerview/>
        </el-header>
        <el-container>
            <el-aside width="20vw" class="hide-aside">
            <dashboard/>
            </el-aside>
            <el-main style="overflow-y: auto; background-color: white; margin-top: 2vh; margin-left: 0.7vw; border-radius: 15px 15px 0 0;">
              <div class="main-content">
                <div class="news-header" style="display: grid; grid-template-columns: 1fr 1fr 2fr 3fr 5fr; align-items: center; font-weight: bold;">
                  <span class="news-number">序号</span>
                  <span class="news-id">ID</span>
                  <span class="news-tags">标签</span>
                  <span class="news-title">新闻标题</span>
                  <span class="news-content">新闻摘要</span>
                </div>

                <div class="news-list">
                  <!-- 循环显示新闻项 -->
                  <div class="news-item" v-for="(newsId, index) in displayedNewsIds" :key="newsId">
                    <div class="news-info" style="display: grid; grid-template-columns: 1fr 1fr 2fr 3fr 4fr 1fr; align-items: center;">
                      <span class="news-number">{{ index + 1 + (this.currentPage - 1) * this.pageSize }}</span>
                      <span class="news-id">{{ newsId }}</span>
                      <span class="news-tags">{{ news_tag1[index + (this.currentPage - 1) * this.pageSize] }} | {{ news_tag2[index + (this.currentPage - 1) * this.pageSize] }}</span>
                      <span class="news-title" :title="news_title[index + (this.currentPage - 1) * this.pageSize]">{{ truncateText(news_title[index + (this.currentPage - 1) * this.pageSize], 5) }}</span>
                      <span class="news-content" :title="news_summary[index + (this.currentPage - 1) * this.pageSize]">{{ truncateText(news_summary[index + (this.currentPage - 1) * this.pageSize], 10) }}</span>  
                      <el-button class="delete-button" size="mini" @click="deleteNews(newsId)">删除</el-button>
                    </div>
                  </div>
                </div>

                <div style="display: flex; justify-content: center;">
                  <el-pagination
                    class="pagination"
                    @current-change="handlePageChange"
                    :current-page="currentPage"
                    :page-size="pageSize"
                    :total="news_id.length"
                    layout="prev, pager, next"
                  />
                </div>
              </div>
            </el-main>
        </el-container>
        </el-container>
    </div>
</template>

<style scoped>
@media (max-width: 768px) { /* 设置适当的最大宽度值 */
  .hide-aside {
    display: none;
  }
  .hide-header {
    display: none;
  }
}

#building{
    background-color:#eee;
    left:0px;
    top:0px;
    width:100vw;			
    height:100vh;		
    position: fixed;
}

.rooter-box{
    position: fixed;
    width:80vw;
    height:100vh;
    left: 10vw;
}

.overflow-container {
  overflow-y: auto;
  max-height: 625px;
}

.overflow-container::-webkit-scrollbar {
  width:0;
}



.news-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(45vw, 1fr));
  gap: 1vh 1vw;
  margin: 20px 0;
}

.news-item {
  border: 1px solid #ccc;
  border-radius: 5px;
}

.news-info {
  display: flex;
  align-items: flex-start;
  height: 10vh;
}

.news-number {
  font-weight: bold;
}

.news-id {
  color: #888;
}

.news-tags {
  color: #888;
}

.news-title {
  margin-top: 5px;
  font-weight: bold;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.news-content {
  margin-top: 5px;
  color: #555;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.delete-button {
  margin-top: 10px;
}
</style>