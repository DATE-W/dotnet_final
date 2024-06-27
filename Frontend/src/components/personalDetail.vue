<!-- 我的动态 v2.0 -->
<template>
    <!-- 上方展示背景图 -->
    <el-container class="main-container">
        <el-main class="user-profile">
            <div class="overflow-container" ref="scrollContainer">
                <div class="bg-theme" :style="{ backgroundImage: 'url(' + backgroundImage + ')' }">
                </div>
                <!-- 遍历动态分组数组，为每个分组创建区域 -->
                <div class="dynamic-group" v-for="(group, groupIndex) in groupedDynamics" :key="groupIndex">
                    <!-- 遍历分组内的动态，为每个动态创建区域 -->
                    <div class="dynamic-item" v-for="(dynamic, index) in group" :key="index">
                        <!-- 显示动态的具体内容 -->
                        <div class="dynamic-content">
                            <!-- 添加 contain 元素 -->
                            <!-- 第一行：用户行为 -->
                            <div class="action-text">{{ getActionText(dynamic) }}</div>
                            <div @click="ClickToDetail(dynamic)"
                                :class="{ 'title-contain-author': dynamic.type !== 'follow', 'user-section': dynamic.type === 'follow' }">
                                <!-- 如果是用户则不用title-contain-author的样式 -->
                                <!-- 第二行：头像+用户名 或 帖子名 -->
                                <div class="avatar-and-username">
                                    <el-avatar v-if="dynamic.type === 'follow'" :src="getUserAvatar(dynamic)"
                                        class="user-avatar"></el-avatar>
                                    <div class="name-text">{{ getNameText(dynamic) }}</div>
                                </div>
                                <!-- 第三行：contain（如果存在） -->
                                <div v-if="dynamic.object.contain" class="contain-text">{{ dynamic.object.contain }}</div>
                                <!-- 第四行：帖子作者（如果存在） -->
                                <div class="avatar-and-author">
                                    <el-avatar v-if="dynamic.object.author" :src="getAuthorAvatar(dynamic)"
                                        class="author-avatar"></el-avatar>
                                    <div v-if="dynamic.object.author" class="author-text">{{ dynamic.object.author }}</div>
                                </div>
                            </div>
                            <!-- 显示动态的时间 -->
                            <div class="dynamic-time">
                                {{ FormatDate(dynamic.time) }}
                            </div>
                        </div>
                    </div>
                </div>
                <div v-if="dynamics.length === 0" class="dynamic-empty-container">
                    <el-empty description="当前暂无动态，请去论坛看看吧!" />
                    <div class="empty-button">
                        <el-button class="dynamic-empty" @click="goToForum" type="text">点击此处前往论坛</el-button>
                    </div>
                </div>
            </div>
        </el-main>
        <el-aside>
            <div class="back-to-top-container">
                <div></div>
                <div class="back-to-top" :class="{ 'animating': isScrollingToTop }" @click="scrollToTop">
                </div>
            </div>
        </el-aside>
    </el-container>
    <!-- 展示我的动态 -->
</template>

<script>
import { ElMessage, ElMessageBox } from 'element-plus';
import axios from 'axios';
export default {
    data() {
        return {
            dynamics: [
            ],
            backgroundImage: '',
            isScrollingToTop: false,
        };
    },
    computed: {
        // 将动态按时间分组的计算属性
        groupedDynamics() {
            const groups = {};
            this.dynamics.forEach(dynamic => {
                const date = dynamic.time.split('T')[0]; // 获取日期部分，格式：YYYY-MM-DD
                if (!groups[date]) {
                    groups[date] = [];
                }
                groups[date].push(dynamic);
            });
            return Object.values(groups);
        },
    },
    mounted() {
        this.ShowAction();
        this.getTheme();
    },
    methods: {
        scrollToTop() {
            // 获取元素的引用
            const container = this.$refs.scrollContainer;
            if (container) {
                this.isScrollingToTop = true;
                const duration = 1000; // 滚动持续时间，单位：毫秒
                const startTime = performance.now();
                const startTop = container.scrollTop;
                const targetTop = 0; // 目标滚动位置（顶部）
                const animateScroll = (timestamp) => {
                    const elapsedTime = timestamp - startTime;
                    if (elapsedTime >= duration) {
                        container.scrollTop = targetTop;
                        this.isScrollingToTop = false;
                        return;
                    }
                    const easeInOutCubic = (t) => t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
                    const scrollProgress = easeInOutCubic(elapsedTime / duration);
                    container.scrollTop = startTop + (targetTop - startTop) * scrollProgress;
                    requestAnimationFrame(animateScroll);
                };
                requestAnimationFrame(animateScroll);
            }
        },
        goToForum() {
            this.$router.push('/forum');
        },
        // 根据动态类型返回相应的文本内容
        getActionText(dynamic) {
            switch (dynamic.type) {
                case 'like':
                    return `您赞同了帖子`;
                case 'follow':
                    return `您关注了用户`;
                case 'comment':
                    return `您评论了帖子`;
                case 'collect':
                    return `您收藏了帖子`;
                case 'publish':
                    return '您发布了帖子';
                default:
                    return '未知操作';
            }
        },
        // 获取用户名或帖子名
        getNameText(dynamic) {
            return dynamic.object.username || dynamic.object.title;
        },
        getUserAvatar(dynamic) {
            if (dynamic.type === 'follow') {
                // 根据需要设置用户头像的 URL
                return 'https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png';
            }
            return null;
        },
        getAuthorAvatar(dynamic) {
            if (dynamic.object.author) {
                // 根据需要设置帖子作者头像的 URL
                return 'https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png';
            }
            return null;
        },
        FormatDate(datetime) {
            const date = new Date(datetime);
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        },
        ClickToDetail(dynamic) {
            if (dynamic.type !== 'follow') {
                this.goToDetail(dynamic.object.post_id);
            }
        },
        goToDetail(postId) {
            console.log(postId);
            this.$router.push({
                path: '/detail',
                query: { clickedPostID: postId }
            });
        },
        async ShowAction() {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/User/userAction', {}, { headers });
                console.log(response);
                this.dynamics = response.data.actions.map(action => {
                    return {
                        type: action.type,
                        time: action.datetime,
                        object: {
                            title: action.title,
                            contain: action.contains,
                            username: action.name,
                            author: action.author,
                            post_id: action.post_id,
                        }
                    };
                });
            } catch (err) {
                ElMessage({
                    message: '服务器错误，获取动态失败',
                    grouping: false,
                    type: 'error',
                });
            }
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
                    message: '服务器错误，获取主题失败',
                    grouping: false,
                    type: 'error',
                })
                return
            }
            console.log(response);
            this.backgroundImage = response.data.image1;
            console.log(this.backgroundImage)
        }
    }
};
</script>

<style scoped>
.overflow-container {
    overflow-y: auto;
    max-height: 82vh;
}

.overflow-container::-webkit-scrollbar {
    width: 0;
}

.bg-theme {
    margin-top: -20px;
    position: relative;
    width: 100%;
    height: 250px;
    background-repeat: no-repeat;
    background-size: cover;
    margin-bottom: 5px;
}

.dynamic-group {
    margin-bottom: 40px;
    /* 调整分组之间的间距,每一组为每天的动态 */
}

.dynamic-item {
    border: 1px solid #ccc;
    margin-bottom: 10px;
    padding: 10px;
}

.dynamic-empty-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100%;
}

.empty-button {
    display: flex;
    justify-content: center;
    margin-top: -30px;
}


.dynamic-empty {
    font-size: 15px;
}

.dynamic-content {
    font-size: 16px;
    margin-bottom: 5px;
}

.action-text {
    font-size: 16px;
    margin-bottom: 5px;
}

.name-text {
    font-size: 18px;
    font-weight: bold;
    margin-bottom: 5px;
}

.title-contain-author {
    background-color: #f0f0f0;
    /* 背景颜色为灰色 */
    padding: 5px;
    /* 添加一些内边距 */
}

.avatar-and-username {
    display: flex;
    align-items: center;
}

.contain-text {
    font-size: 15px;
    margin-bottom: 5px;
}

.author-text {
    font-size: 14px;
    text-align: right;
    margin-top: 1px;
}

.dynamic-time {
    font-size: 12px;
    color: #999;
}

.avatar-and-author {
    display: flex;
    justify-content: flex-end;
}

.author-avatar {
    width: 25px;
    height: 25px;
    margin-right: 5px;
}

.user-avatar {
    width: 25px;
    height: 25px;
    margin-right: 5px;
}

/* author头像 */

.scroll-to-top-btn {
    position: fixed;
    bottom: 20px;
    right: 20px;
    width: 50px;
    height: 50px;
    cursor: pointer;
    overflow: hidden;
    z-index: 999;
}

.image-container img {
    width: 100%;
    height: 100%;
    display: block;
    transition: opacity 0.3s;
}

.back-to-top-container {
    display: flex;
    position: relative;
    width: 300px;
    height: 620px;
}

.back-to-top {
    cursor: pointer;
    top: 0;
    width: 70px;
    height: 590px;
    margin-left: 35%;
    background: url(./src/assets/img/scroll_cat.png);
    transition: transform 0.5s ease-in-out, opacity 0.5s ease-in-out;
    opacity: 1;
}

.back-to-top.animating {
    transform: translateY(-200px);
    opacity: 1;
}
</style>