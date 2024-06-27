<script>
import axios from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';
import dashboard from './AdminDashBoard.vue';
import headerview from './AdminNav.vue';
export default {
    components: {
        dashboard: dashboard,
        headerview: headerview,
    },
    mounted() {
        this.getReportedPost();
        this.getAllPost();
    },
    data() {
        return {
            allPost: [],
            reportedPost: [],
            tittle: "",
            contains: "",
        };
    },
    methods: {
        analyse_date(date) {
            // 创建一个 Date 对象来解析时间字符串
            const dateObject = new Date(date);
            // 提取年、月和日
            const year = dateObject.getFullYear();
            const month = String(dateObject.getMonth() + 1).padStart(2, "0"); // 月份是从 0 开始的，需要加1
            const day = String(dateObject.getDate()).padStart(2, "0");
            // 构建目标格式的日期字符串
            const formattedDate = `${year}-${month}-${day}`;
            return formattedDate;
        },
        async killPost(index) {
            console.log("killPost")
            let response
            try {
                response = await axios.post('api/report/agreeReport', {
                    reporter_id: this.reportedPost[index].reporter_id,
                    post_id: this.reportedPost[index].post_id,
                    reply: " ",
                });
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
            if (response.data.ok == 'yes') {
                ElMessage.success("成功删除本帖");
                location.reload();
            }
            else {
                ElMessage.error("未找到相关举报信息");
            }
            return
        },
        async killPostByAdmin(index) {
            console.log("killPostByAdmin")
            console.log(this.allPost[index].post_id)
            let response
            try {
                response = await axios.post('/api/Admin/BanPostbyID', {
                    post_id: this.allPost[index].post_id,
                });
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
            this.allPost[index].isbanned = 1;
            //location.reload();
            return
        },
        async toPost(postId) {
            console.log("getPost")
            let response
            try {
                response = await axios.post('/api/Post/getPostInfo', {
                    post_id: postId,
                });
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
            if (response.data.ok == 'yes') {
                this.tittle = response.data.value.title;
                this.contains = response.data.value.contains;
            }
            else {
                ElMessage.error("获取帖子信息失败");
            }
            ElMessageBox.alert(this.contains, this.tittle, {
                confirmButtonText: 'OK',
            })
            return
        },
        async getReportedPost() {
            console.log("start get reported post")
            let response
            try {
                response = await axios.get('api/report/getReportInfo');
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
                ElMessage.error("获取被举报帖子列表失败");
            } else if (response.data.ok == 'yes') {
                // 遍历传来的数据并进行转换
                response.data.value.forEach(item => {
                    const convertedItem = {
                        post_id: item.post_id,
                        title: item.title,
                        publisherName: item.publisherName,
                        reporterName: item.reporterName,
                        reporter_id: item.reporter_id,
                        description: item.description || "没有提供原因"
                    };
                    // 将转换后的数据添加到 reportedPost 数组中
                    this.reportedPost.push(convertedItem);
                    this.reportedPost.sort((a, b) => a.post_id - b.post_id);
                });
            }
            return
        },
        async getAllPost() {
            console.log("start get all post")
            let response
            try {
                response = await axios.get('/api/Admin/GetAllPost');
            } catch (err) {
                console.error(err);
                if (err.response.data.result == 'fail') {
                    ElMessage.error(err.response.data.msg)
                } else {
                    ElMessage.error("未知错误")
                }
                return
            }
            //console.log("AllPost - JSON.stringify(response) = "+JSON.stringify(response, null, 2))
            // 遍历传来的数据并进行转换
            response.data.forEach(item => {
                const convertedItem = {
                    post_id: item.post_id,
                    author_avatar: item.author_avatar === "/" ? "https://cube.elemecdn.com/9/c2/f0ee8a3c7c9638a54940382568c9dpng.png" : item.author_avatar,
                    author_name: item.author_name,
                    title: item.title,
                    contains: item.contains.slice(0, 60),
                    publishtime: this.analyse_date(item.publishtime),
                    approvalnum: item.approvalnum,
                    isbanned: item.isbanned,
                };
                // 将转换后的数据添加到 reportedPost 数组中
                this.allPost.push(convertedItem);
                this.allPost.sort((a, b) => a.post_id - b.post_id);
            });
            return
        },
        async cancelReport(index) {
            console.log("cancelReport")
            let response
            try {
                response = await axios.post('api/report/cancelReport', {
                    reporter_id: this.reportedPost[index].reporter_id,
                    post_id: this.reportedPost[index].post_id,
                    reply: " ",
                });
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
            location.reload();
            return
        },
    }
}
</script>

<template>
    <div id="building">
        <el-container class="rooter-box">
            <el-header class="hide-header">
                <headerview />
            </el-header>
            <el-container>
                <el-aside width="20vw" class="hide-aside">
                    <dashboard />
                </el-aside>
                <el-main
                    style="overflow: hidden;background-color:white;margin-top: 2vh;margin-left: 0.7vw;border-radius: 15px 15px 0 0;">
                    <el-table :data="reportedPost" border height="300" style="width: 100%;border-radius: 10px;">
                        <el-table-column :label="`被举报帖子列表`" align="center">
                            <el-table-column align="center" prop="post_id" label="帖子Id" width="100" />
                            <el-table-column prop="title" label="帖子标题" width="150" />
                            <el-table-column prop="publisherName" label="发帖人" width="100" />
                            <el-table-column prop="reporterName" label="举报人" width="100" />
                            <el-table-column prop="description" label="举报理由" width="200" />
                            <el-table-column fixed="right" label="操作">
                                <template #default="scope">
                                    <el-button link type="primary" size="small"
                                        @click="toPost(reportedPost[scope.$index].post_id)">查看帖子</el-button>
                                    <el-button link type="primary" size="small"
                                        @click="killPost(scope.$index)">删除帖子</el-button>
                                    <el-button link type="primary" size="small"
                                        @click="cancelReport(scope.$index)">取消举报</el-button>
                                </template>
                            </el-table-column>
                        </el-table-column>
                    </el-table>
                    <el-container class="all-post" style="overflow-y: auto;overflow-x: hidden;">
                        <span
                            style="position: relative;left:25vw;font-weight: bold;color: #404A57;margin-top: 0.5vh;">全部帖子列表</span>
                        <el-container v-for="(post, index) in allPost">
                            <el-container class="single-post">
                                <el-container style="display: flex;justify-content: center;padding-top: 1vh;">
                                    <span>{{ post.title }}</span>
                                </el-container>
                                <el-container
                                    style="display: flex;justify-content: left;flex-direction: column;padding-top: 1vh;">
                                    <span style="margin-left: 1vw;">{{ post.contains }}</span>
                                </el-container>
                                <el-container style="padding-top: 1vh;">
                                    <span style="margin-left: 3vw;">获赞数：{{ post.approvalnum }}</span>
                                    <span style="color:rgb(41, 93, 151);margin-left: 10vw;">{{ post.author_name }}</span>
                                    发布于{{ post.publishtime }}
                                </el-container>
                            </el-container>
                            <el-container class="single-author">
                                <img style="width:4vw;height:4vw;" :src="post.author_avatar" />
                                <span
                                    style="font-family: KaiTi;font-size:1.25rem;color:rgb(41, 93, 151);margin-top: 1vh;font-size: 1rem;">{{
                                        post.author_name }}</span>
                            </el-container>
                            <el-container class="single-author" @click="killPostByAdmin(index)">
                                <span v-if="post.isbanned == 0"
                                    style="color: rgb(61, 106, 230);font-weight: bold;">删除帖子</span>
                                <span v-if="post.isbanned == 1" style="color: rgb(218, 52, 52);font-weight: bold;">本帖已封</span>
                            </el-container>
                        </el-container>
                    </el-container>
                </el-main>
            </el-container>
        </el-container>
    </div>
</template>

<style scoped>
@media (max-width: 768px) {

    /* 设置适当的最大宽度值 */
    .hide-aside {
        display: none;
    }

    .hide-header {
        display: none;
    }
}

#building {
    background-color: #eee;
    left: 0px;
    top: 0px;
    width: 100vw;
    height: 100vh;
    position: fixed;
}

.rooter-box {
    position: fixed;
    width: 80vw;
    height: 100vh;
    left: 10vw;
}

.all-post {
    display: flex;
    flex-direction: column;
    width: 57vw;
    height: 40vh;
    margin-top: 3vh;
    border-radius: 10px;
    background-color: #f6f6f6;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.single-post {
    background-color: white;
    width: 35vw;
    height: 15vh;
    margin-top: 2vh;
    display: flex;
    flex-direction: column;
    border: 1px dashed;
    border-radius: 10px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.single-author {
    background-color: white;
    width: 5vw;
    height: 15vh;
    margin-top: 2vh;
    margin-left: 1vw;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    border: 1px dashed;
    border-radius: 10px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    cursor: pointer;
}

.single-post:hover {
    box-shadow: 0 8px 12px rgba(0, 0, 0, 0.2);
    /* 鼠标悬浮时的阴影效果 */
}

.single-author:hover {
    box-shadow: 0 8px 12px rgba(0, 0, 0, 0.2);
    /* 鼠标悬浮时的阴影效果 */
    background-color: rgb(240, 241, 241);
    /* 鼠标悬浮时的背景颜色 */
}</style>