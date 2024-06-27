<!--论坛页面v3.0-->
<template>
    <div>
        <el-container>
            <my-nav></my-nav>
            <el-header class="header-container">
                <!-- 论坛标题 -->
                <div class="forum-title" ref="forumTitle">
                    <a>同济绿茵&nbsp;&nbsp;&nbsp;&nbsp;论坛</a>
                </div>
            </el-header>
            <div class="line"></div>
            <el-container>
                <el-aside>
                    <!-- 发帖按钮 -->
                    <div class="post-button-container">
                        <el-button class="post-button" @click="redirectToEditPost" round>
                            <text class="post-buttontext">{{ isAccount ? '点此发帖' : '点此登录' }}</text>
                        </el-button>
                    </div>
                    <div class="select-tips">
                        <span class="tips-text">点击下方不同联赛,查看对应主题帖...</span>
                    </div>
                    <el-menu class="tags-container" @select="SelectLeftTag" default-active="1">
                        <el-menu-item index="1">
                            <img :src="leagues_logo[0].logo" class="logo-show">
                            <span class="tag-text">全部赛事</span>
                        </el-menu-item>
                        <el-menu-item index="2">
                            <img :src="leagues_logo[1].logo" class="logo-show">
                            <span class="tag-text">英超</span>
                        </el-menu-item>
                        <el-menu-item index="3">
                            <img :src="leagues_logo[2].logo" class="logo-show">
                            <span class="tag-text">西甲</span>
                        </el-menu-item>
                        <el-menu-item index="4">
                            <img :src="leagues_logo[3].logo" class="logo-show">
                            <span class="tag-text">意甲</span>
                        </el-menu-item>
                        <el-menu-item index="5">
                            <img :src="leagues_logo[4].logo" class="logo-show">
                            <span class="tag-text">德甲</span>
                        </el-menu-item>
                        <el-menu-item index="6">
                            <img :src="leagues_logo[5].logo" class="logo-show">
                            <span class="tag-text">法甲</span>
                        </el-menu-item>
                        <el-menu-item index="7">
                            <img :src="leagues_logo[6].logo" class="logo-show">
                            <span class="tag-text">中超</span>
                        </el-menu-item>
                    </el-menu>
                </el-aside>
                <el-main>
                    <div class="up-container">
                        <!-- 搜索和排序 -->
                        <el-icon class="search-icon">
                            <Search />
                        </el-icon>
                        <el-input class="search-input" v-model="searchKeyword" placeholder="请输入关键词"
                            @keyup.enter.native="SearchForPosts(1, this.pageSize, this.currentTag, this.searchKeyword)"></el-input>
                        <el-button v-if="ShowReturnButton" icon="ArrowLeft" @click="ReturnToBegin"></el-button>
                        <div class="sort-container">
                            <el-button v-if="ShowSortButtons" :class="{ 'sort-button': sortbytime }"
                                @click="changeSort('time')">按时间排序</el-button>
                            <el-button v-if="ShowSortButtons" :class="{ 'sort-button': sortbylike }"
                                @click="changeSort('like')">按热度排序</el-button>
                        </div>
                    </div>
                    <!-- 展示帖子 -->
                    <div class="card-container">
                        <el-row>
                            <el-col :span="24" v-for="(title, index) in post_title" :key="index">
                                <el-card shadow="hover" class="post-card" @click="goToDetail(post_id[index])">
                                    <div class="post-title">{{ truncatedTitle(title) }}</div>
                                    <div class="post-contain">{{ truncatedContent(post_contains[index]) }}</div>
                                    <div class="post-footer">
                                        <div class="approval-collect">
                                            <el-icon>
                                                <CircleCheck />
                                            </el-icon>
                                            <span class="post-approval">{{ post_approval[index] }}</span>
                                            <el-icon>
                                                <Star />
                                            </el-icon>
                                            <span>{{ post_collect[index] }}</span>
                                        </div>
                                        <div class="post-author">
                                            <el-icon>
                                                <Avatar />
                                            </el-icon>
                                            {{ post_author[index] }}
                                        </div>
                                    </div>
                                </el-card>
                            </el-col>
                        </el-row>
                    </div>
                    <!-- 分页栏 -->
                    <div class="page-container" v-if="this.showPage">
                        <el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
                            layout="prev, pager, next, jumper" :total="totalPosts"></el-pagination>
                    </div>
                </el-main>
            </el-container>
        </el-container>
    </div>
</template>


<style scoped>
@import '../assets/font/font.css';

.el-aside {
    background-color: aliceblue;
    height: 650px;
}

.el-main {
    background-color: aliceblue;
    height: 650px;
}


.header-container {
    /* 顶部论坛标题 */
    height: 15vh;
    background-size: cover;
}

.line {
    background: #000000;
    box-shadow: 0px 4px 4px 0px rgba(0, 0, 0, 0.25) inset;
    position: relative;
    left: 10%;
    width: 80%;
    height: 3px;
}


.forum-title {
    /* 论坛艺术字 */
    position: relative;
    top: 20%;
    flex-shrink: 0;
    text-align: center;
    font-family: artfont;
    font-size: 50px;
    font-style: normal;
    font-weight: 400;
    line-height: 10vh;
}

.line {
    /* 标题和论坛主体的分割线 */
    background: #000000;
    box-shadow: 0px 4px 4px 0px rgba(0, 0, 0, 0.25) inset;
    position: relative;
    left: 10%;
    width: 80%;
    height: 3px;
}

.aside-container {
    /* 用来使按钮居中 */
    display: flex;
    flex-direction: column;
    align-items: center;
}

.post-button-container {
    /* 用来使按钮居中 */
    margin-bottom: 0.3vw;
    flex-grow: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
}

.post-button {
    /* 发帖按钮 */
    margin-top: 4vh;
    margin-bottom: 0.3vw;
    border-radius: 8vw;
    background-color: #3098f9;
    box-shadow: 1px 1px 5px 0px rgba(0, 0, 0, 0.25);
    width: 14vw;
    height: 3vw;
}

.post-buttontext {
    /* 发帖按钮 */
    color: rgb(255, 255, 255);
    font-family: Ubuntu;
    font-size: 1.2rem;
    font-style: normal;
    font-weight: 700;
    line-height: normal;
    letter-spacing: 0.8px;
}

.tags-container {
    /* 左侧选择联赛菜单 */
    margin-top: 15px;
    background-color: #F5F7FA;
}

.select-tips {
    /* 提示选择联赛 */
    margin-top: 10px;
    margin-bottom: 10px;
    flex-grow: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
}

.tips-text {
    font-size: 10px;
    color: cornflowerblue;
}

.el-menu-item {
    /* 菜单元素悬浮 */
    transition: background-color 0.8s ease;
}

.el-menu-item:hover {
    /* 菜单元素悬浮 */
    background-color: #8cf1fc;
}

.logo-show {
    /* 展示tag的logo */
    width: 30px;
    height: 30px;
    border-radius: 50%;
    margin-left: 40px;
    margin-right: 10px;
}

.tag-text {
    font-size: 20px;
    font-weight: bold;
}

.up-container {
    /* 搜索框和排序 */
    margin-top: -5px;
    display: flex;
    align-items: center;
}

.search-input {
    /* 搜索框 */
    width: 30%;
}

.search-icon {
    /* 搜索图标 */
    font-size: 24px;
}

.sort-container {
    /* 排序按钮 */
    margin-left: 575px;
}

.sort-button {
    /* 排序按钮 */
    color: orange;
}

.card-container {
    /* 全部帖子的容器 */
    margin-left: 130px;
    margin-top: 15px;
}

.post-card {
    /* 展示帖子的card */
    width: 900px;
    height: 100px;
    margin-top: 10px;
    margin-bottom: 10px;
}

.post-title {
    margin-top: -10px;
    font-size: 1.3rem;
    font-style: normal;
    font-weight: 400;
    color: #005ce6;
}

.post-contain {
    margin-top: 8px;
    font-size: 1.0rem;
    font-style: normal;
    font-weight: 400;
}

.post-footer {
    margin-top: 5px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: 10px;
    color: #888;
    font-size: 0.8rem;
}

.post-approval {
    margin-right: 10px;
}

.post-author {
    align-items: center;
    display: flex;
}

.page-container {
    position: absolute;
    bottom: 0px;
    left: 55%;
    transform: translateX(-50%);
}
</style>

<script>
import MyNav from './nav.vue';
import axios from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';
import anime from 'animejs';
export default {
    components: {
        'my-nav': MyNav
    },
    data() {
        return {
            isAccount: false,  //判断是否登录 初始为false
            isEmpty: false,  //判断是否没有帖子
            sortbytime: true,  //按时间排序帖子 默认为true
            sortbylike: false,  //按热度排序
            ShowReturnButton: false,  //搜索框右侧展示回退
            ShowSortButtons: true,  //选择排序方式按钮
            searchKeyword: '', // 搜索关键词
            currentPage: 1,  //当前页数
            pageSize: 4,  //每页4个帖子
            totalPosts: 0,  //帖子数
            showPage: false,  //展示分页栏
            post_id: [],  //存储返回的帖子id
            post_title: [],  //存储返回的帖子标题
            post_contains: [],  //存储返回的帖子内容
            post_author: [],  //存储返回的帖子作者
            post_approval: [],  //存储帖子点赞数
            post_collect: [],  //存储帖子收藏数
            currentTag: 'ALL',  //向后端传递当前页面的帖子类型 初始为全部 不受tag影响
            leagues_logo: [
                { "logo": "/src/assets/img/football_logo.png" },
                { "logo": "/src/assets/img/pmlogo.png" },
                { "logo": "/src/assets/img/lllogo.png" },
                { "logo": "/src/assets/img/salogo.png" },
                { "logo": "/src/assets/img/bllogo.png" },
                { "logo": "/src/assets/img/le1logo.png" },
                { "logo": "/src/assets/img/cslogo.png" },
            ],
        }
    },
    mounted() {
        this.JudgeAccount();//判断是否登录
        this.getPostsbyTime(1, this.pageSize, this.currentTag);//按时间获取帖子
        this.getPostNum();//获取所有的帖子的总数
        this.animateForumTitle();
    },
    methods: {
        truncatedTitle(title) {
            return title.length > 12 ? title.slice(0, 12) + '...' : title;
        },
        truncatedContent(content) {
            return content.length > 50 ? content.slice(0, 50) + '...' : content;
        },
        SelectLeftTag(index) {
            switch (index) {
                case '1':
                    this.currentTag = 'ALL';
                    break;
                case '2':
                    this.currentTag = '英超';
                    break;
                case '3':
                    this.currentTag = '西甲';
                    break;
                case '4':
                    this.currentTag = '意甲';
                    break;
                case '5':
                    this.currentTag = '德甲';
                    break;
                case '6':
                    this.currentTag = '法甲';
                    break;
                case '7':
                    this.currentTag = '中超';
                    break;
                default:
                    break;
            };
            if (this.sortbytime)
                this.getPostsbyTime(1, this.pageSize, this.currentTag);
            else
                this.getPostsbyLike(1, this.pageSize, this.currentTag);
        },
        animateForumTitle() {
            const forumTitle = this.$refs.forumTitle; // 获取标题元素
            forumTitle.innerHTML = forumTitle.textContent.replace(/\S/g, "<span class='letter'>$&</span>");

            // 使用Anime.js创建动画效果
            anime.timeline({ loop: false })
                .add({
                    targets: '.letter',
                    scale: [0.3, 1],
                    opacity: [0, 1],
                    translateZ: 0,
                    easing: 'easeOutExpo',
                    duration: 600,
                    delay: (el, i) => 70 * (i + 1)
                })
                .add({
                    targets: '.line',
                    scaleX: [0, 1],
                    opacity: [0.5, 1],
                    easing: 'easeOutExpo',
                    duration: 700,
                    offset: '-=875',
                    delay: (el, i, l) => 80 * (l - i)
                })
                .add({
                    targets: '.ml1',
                    opacity: 0,
                    duration: 1000,
                    easing: 'easeOutExpo',
                    delay: 1000
                });
        },
        redirectToEditPost() {
            //进入发帖页
            if (this.isAccount)
                this.$router.push('/EditPost');
            else {
                this.$router.push('/signin');
            }
        },
        handlePageChange(currentPage) {
            //换页
            this.currentPage = currentPage;
            if (this.sortbytime)
                this.getPostsbyTime(this.currentPage, this.pageSize, this.currentTag);
            else
                this.getPostsbyLike(this.currentPage, this.pageSize, this.currentTag);
        },
        goToDetail(postId) {
            //进入对应帖子的详情页
            this.$router.push({
                path: '/detail',
                query: { clickedPostID: postId }
            });
        },
        changeSort(button) {
            //选择排序方式
            if (button === 'time') {
                if (this.sortbylike)
                    this.getPostsbyTime(1, this.pageSize, this.currentTag);
                this.sortbytime = true;
                this.sortbylike = false;
            }
            else if (button === 'like') {
                if (this.sortbytime)
                    this.getPostsbyLike(1, this.pageSize, this.currentTag);
                this.sortbytime = false;
                this.sortbylike = true;
            }
        },
        async JudgeAccount() {
            //判断是否有账号
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/UserToken/UserToken', {}, { headers })
            } catch (err) {
                if (err.response.data.result == 'fail') {
                    ElMessage({
                        message: err.response.data.msg,
                        grouping: false,
                        type: 'error',
                    })
                } else {
                    return
                }
                return
            }
            if (response.data.ok == 'yes') {
                this.isAccount = true;
            }
            else {
                this.isAccount = false;
            }
        },
        async getPostNum() {
            //获取帖子总数
            let response
            try {
                response = await axios({
                    method: 'GET',
                    url: '/api/Forum/GetPostNum',
                })
            } catch (err) {
                ElMessage({
                    message: '服务器错误，获取帖子总数失败',
                    grouping: false,
                    type: 'error',
                })
            }
            //console.log(response.data.totalPostsCount);
            if (response.data.totalPostsCount) {
                this.totalPosts = response.data.totalPostsCount; // 将帖子的总数保存到data中的totalItems中
                if (this.totalPosts) {
                    this.isEmpty = false;
                } else {
                    this.isEmpty = true;
                };
                this.showPage = true;
            } else {
                ElMessage({
                    message: '后端返回的帖子总数格式错误',
                    grouping: false,
                    type: 'error',
                });
            }
        },
        async getPostsbyTime(pageNumber, pageSize, currentTag) {
            //时间排序
            let response
            try {
                response = await axios.post('/api/Forum/GetPostbyOrder', {
                    page: pageNumber,
                    count: pageSize,
                    tag: String(currentTag),
                }, {})
            } catch (err) {
                ElMessage({
                    message: '服务器错误，按时间获取帖子失败',
                    grouping: false,
                    type: 'error',
                });
            }
            //console.log('response:', response);
            this.post_id = [];
            this.post_title = [];
            this.post_contains = [];
            this.post_author = [];
            this.post_approval = [];
            this.post_collect = [];
            this.totalPosts = response.data.count;
            if (response.data.postInfoJsons && Array.isArray(response.data.postInfoJsons)) {
                response.data.postInfoJsons.forEach((postInfo) => {
                    this.post_id.push(postInfo.post_id);
                    this.post_title.push(postInfo.title);
                    this.post_contains.push(postInfo.contains);
                    this.post_author.push(postInfo.author);
                    this.post_approval.push(postInfo.approvalNum);
                    this.post_collect.push(postInfo.collectNum);
                });
            }
            else {
                ElMessage({
                    message: '后端返回的帖子数据格式错误',
                    grouping: false,
                    type: 'error',
                });
            }
        },
        async getPostsbyLike(pageNumber, pageSize, currentTag) {
            //热度排序
            let response
            try {
                response = await axios.post('/api/Forum/GetPostbyLike', {
                    page: pageNumber,
                    count: pageSize,
                    tag: String(currentTag),
                }, {})
            } catch (err) {
                ElMessage({
                    message: '服务器错误，按热度获取帖子失败',
                    grouping: false,
                    type: 'error',
                });
            }
            this.post_id = [];
            this.post_title = [];
            this.post_contains = [];
            this.post_author = [];
            this.post_approval = [];
            this.post_collect = [];
            this.totalPosts = response.data.count;
            if (response.data.postInfoJsons && Array.isArray(response.data.postInfoJsons)) {
                response.data.postInfoJsons.forEach((postInfo) => {
                    this.post_id.push(postInfo.post_id);
                    this.post_title.push(postInfo.title);
                    this.post_contains.push(postInfo.contains);
                    this.post_author.push(postInfo.author);
                    this.post_approval.push(postInfo.approvalNum);
                    this.post_collect.push(postInfo.collectNum);
                });
            }
            else {
                ElMessage({
                    message: '后端返回的帖子数据格式错误',
                    grouping: false,
                    type: 'error',
                });
            }
        },
        async SearchForPosts(pageNumber, pageSize, currentTag, keyword) {
            //搜索帖子 搜索前不展示回退按钮 搜索后展示回退 不展示分类
            this.ShowReturnButton = true;
            this.ShowSortButtons = false;
            let response
            try {
                response = await axios.post('/api/Forum/GetPostbySearch', {
                    page: pageNumber,
                    count: pageSize,
                    tag: String(currentTag),
                    key: String(keyword),
                }, {})
            } catch (err) {
                ElMessage({
                    message: '服务器错误，搜索帖子失败',
                    grouping: false,
                    type: 'error',
                });
            }
            this.post_id = [];
            this.post_title = [];
            this.post_contains = [];
            this.post_author = [];
            this.post_approval = [];
            this.post_collect = [];
            this.totalPosts = response.data.count;
            if (response.data.postInfoJsons && Array.isArray(response.data.postInfoJsons)) {
                response.data.postInfoJsons.forEach((postInfo) => {
                    this.post_id.push(postInfo.post_id);
                    this.post_title.push(postInfo.title);
                    this.post_contains.push(postInfo.contains);
                    this.post_author.push(postInfo.author);
                    this.post_approval.push(postInfo.approvalNum);
                    this.post_collect.push(postInfo.collectNum);
                });
            }
            else {
                ElMessage({
                    message: '后端返回的帖子数据格式错误',
                    grouping: false,
                    type: 'error',
                });
            }
            console.log('搜索关键词:', this.searchKeyword);
        },
        ReturnToBegin() {
            //搜索回退
            this.ShowReturnButton = false;
            this.ShowSortButtons = true;
            if (this.sortbytime) {
                this.getPostsbyTime(1, this.pageSize, this.currentTag);
            } else {
                this.getPostsbyLike(1, this.pageSize, this.currentTag);
            }
        }
    },
}
</script>
