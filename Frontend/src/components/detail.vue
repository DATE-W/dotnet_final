<script>
import MyNav from './nav.vue';
import axios from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';

export default {
    components: {
        'my-nav': MyNav
    },
    created() {
        this.setData();
        //this.JudgeAccount();
        this.GetPostDetail(this.$route.query.clickedPostID);
        this.getAllPost();
        console.log("post_id = " + this.$route.query.clickedPostID);
    },
    data() {
        return {

            isAccount: false,

            //用来判断几个按钮的初始情况
            isApproved: false,
            isCollected: false,
            isReported: false,
            isFollowed: false,

            //举报相关
            dialogFormVisible: false,
            formLabelWidth: '140px',
            report_descriptions: "",

            //帖子信息相关
            title: "Default Title",
            uText: "Default Text",
            pic: [],
            date: "Default Date",
            approvalNum: 0,

            //发帖人信息
            author_id: 0,
            avatar: "",
            uName: "Default Name",

            //展示信息
            show_name: "",
            show_avatar: "",
            show_uft: "",
            show_signature: "",
            show_follownum: 0,
            show_followednum: 0,
            show_likenum: 0,

            //评论相关
            jId: [],
            jAvatar: [],
            jNames: [],
            jTexts: [],
            jDates: [],

            userJudge: "",

            currentPostId: 0,

            allPost: [],
        }
    },
    methods: {
        setData() {
            //用来判断几个按钮的初始情况
            this.isApproved = false
            this.isCollected = false
            this.isReported = false
            this.isFollowed = false

            //举报相关
            this.dialogFormVisible = false
            this.report_descriptions = ""

            //帖子信息相关
            this.title = ""
            this.uText = ""
            this.pic = []
            this.date = ""
            this.approvalNum = 0

            //发帖人信息
            this.author_id = 0
            this.avatar = ""
            this.uName = ""

            //展示信息
            this.show_name = ""
            this.show_avatar = ""
            this.show_uft = ""
            this.show_signature = ""
            this.show_follownum = 0
            this.show_followednum = 0
            this.show_likenum = 0

            //评论相关
            this.jId = []
            this.jAvatar = []
            this.jNames = []
            this.jTexts = []
            this.jDates = []

            this.userJudge = ""

            this.currentPostId = 0
            return
        },
        // async JudgeAccount() {
        //     const token = localStorage.getItem('token');
        //     let response
        //     try {
        //         const headers = {
        //             Authorization: `Bearer ${token}`,
        //         };
        //         response = await axios.post('/api/UserToken/UserToken', {}, { headers })
        //     } catch (err) {
        //         if (err.response.data.result == 'fail') {
        //             ElMessage({
        //                 message: err.response.data.msg,
        //                 grouping: false,
        //                 type: 'error',
        //             })
        //         } else {
        //             ElMessage({
        //                 message: '未知错误',
        //                 grouping: false,
        //                 type: 'error',
        //             })
        //             return
        //         }
        //         return
        //     }
        //     if (response.data.ok == 'yes') {
        //         this.isAccount = true;  
        //     }
        //     console.log("isAccount = " + this.isAccount);
        //     return;
        // },
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
        async GetPostDetail(post_id) {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/Forum/PostInfo', {
                    post_id: post_id,
                }, { headers });
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
            console.log("PostInfo - JSON.stringify(response) = " + JSON.stringify(response, null, 2))
            if (response.data.ok == 'no') {
                this.$router.push('/signin');
                ElMessage.error("请先登录");
            } else {
                this.title = response.data.title;
                this.avatar = response.data.avatar;
                this.author_id = response.data.author_id;
                this.uName = response.data.name;
                this.uText = response.data.contains;
                this.date = this.analyse_date(response.data.publishDateTime);
                if (response.data.islike == 1) {
                    this.isApproved = true
                }
                if (response.data.iscollect == 1) {
                    this.isCollected = true
                }
                if (response.data.isfollow == 1) {
                    this.isFollowed = true
                }
                this.approvalNum = response.data.approvalNum;
                if (this.getLength(response.data.comments) == 0) {
                    this.jAvatar.push('https://cube.elemecdn.com/9/c2/f0ee8a3c7c9638a54940382568c9dpng.png')
                    this.jNames.push("亲爱的用户")
                    this.jTexts.push("暂无评论哟，快来占领评论区吧！")
                }
                response.data.comments.forEach(jInfo => {
                    if (jInfo.avatar != "/") {
                        this.jAvatar.push(jInfo.avatar);
                    } else {
                        this.jAvatar.push('https://cube.elemecdn.com/9/c2/f0ee8a3c7c9638a54940382568c9dpng.png');
                    }
                    this.jId.push(jInfo.user_id);
                    this.jNames.push(jInfo.userName);
                    this.jTexts.push(jInfo.contains);
                    this.jDates.push(this.analyse_date(jInfo.publishDateTime));
                });
                response.data.pic.forEach(item => {
                    this.pic.push("http://110.40.206.206/" + item);
                    console.log("pic = " + this.pic);
                });
            }
            return;
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

            // 遍历传来的数据并进行转换
            response.data.forEach(item => {
                const convertedItem = {
                    post_id: item.post_id,
                    title: item.title.slice(0, 7),
                    contains: item.contains.slice(0, 14),
                    author_name: item.author_name,
                    publishtime: this.analyse_date(item.publishtime),
                    approvalnum: item.approvalnum
                };
                // 将转换后的数据添加到 allPost 数组中
                this.allPost.push(convertedItem);
                this.allPost.sort((b, a) => a.approvalnum - b.approvalnum);
                // 保留前四个元素
                this.allPost = this.allPost.slice(0, 5);
            });
            return
        },
        async approvePost() {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/Forum/Like', {
                    post_id: this.$route.query.clickedPostID,
                }, { headers });
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
                if (this.isApproved == false) {
                    ElMessage.success("点赞成功");
                } else {
                    ElMessage.success("取消点赞成功");
                }

            } else {
                ElMessage.error("点赞失败");
            }
            this.isApproved = !this.isApproved;
            if (this.isApproved == false) {
                this.approvalNum--;
            } else {
                this.approvalNum++;
            }
            console.log("isApproved2 = " + this.isApproved);
            return;
        },
        async PostJudge() {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/Forum/NewComment', {
                    post_id: this.$route.query.clickedPostID,
                    contains: String(this.userJudge),
                }, { headers });
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
            console.log("发送评论 " + response.data.value);
            this.userJudge = "";
            if (response.data.ok == 'yes') {
                window.location.reload();
            }
            return;
        },
        async collectPost() {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/Forum/collect', {
                    post_id: this.$route.query.clickedPostID,
                }, { headers });
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
                if (this.isCollected == false) {
                    ElMessage.success("收藏成功");
                } else {
                    ElMessage.success("取消收藏成功");
                }

                this.isCollected = !this.isCollected;
            } else {
                ElMessage.error("收藏失败");
            }
            return;
        },
        async reportPost() {
            console.log("send report request")
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/Forum/report', {
                    post_id: this.$route.query.clickedPostID,
                    descriptions: String(this.report_descriptions),
                }, { headers });
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
            console.log(response.data.value)
            if (response.data.ok == 'yes') {
                ElMessage.success("举报成功");
            } else if (response.data.ok == 'no') {
                ElMessage.error("举报失败，请勿多次举报。");
            }
            this.report_descriptions = "";
            return
        },
        async follow() {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/Forum/follow', {
                    post_id: this.$route.query.clickedPostID,
                }, { headers });
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
                if (this.isFollowed == false) {
                    ElMessage.success("关注成功");
                } else {
                    ElMessage.success("取消关注成功");
                }
                this.isFollowed = !this.isFollowed;
            } else {
                ElMessage.error("关注失败");
            }
            return
        },
        goBack() {
            if (this.$route.query.fromAdmin == 1) {
                this.$router.push('/AdminMain');
            } else {
                this.$router.push('/forum');
            }
            return
        },
        async followJudger() {

        },
        async showUserInfo(id, mode) {
            console.log("show user info")
            console.log("id = " + id)
            console.log(mode)
            let response
            try {
                response = await axios.post('/api/User/UserInfo', {
                    author_id: id,
                });
            } catch (err) {
                if (err.response.result == 'fail') {
                    ElMessage({
                        message: err.response.msg,
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
            if (response.data.ok == 'no') {
                ElMessage.error("获取发帖人信息失败");
            } else {
                this.show_name = response.data.name;
                if (response.data.avatar != "/") {
                    this.show_avatar = response.data.avatar;
                } else {
                    this.show_avatar = 'https://cube.elemecdn.com/9/c2/f0ee8a3c7c9638a54940382568c9dpng.png';
                }
                this.show_uft = response.data.uft;
                this.show_signature = response.data.signature;
                this.show_follownum = response.data.follownum;
                this.show_followednum = response.data.followednum;
                this.show_likenum = response.data.likenum;
            }
            const confirmText = this.isFollowed ? '取消关注' : '关注';
            const userInfoHTML = `
    <div style="padding: 10px;">
      <div style="display: flex; align-items: center;">
        <div style="margin-right: 10px;">
          <img src="${this.show_avatar}" alt="User Avatar" style="width: 50px; height: 50px; border-radius: 50%;" />
        </div>
        <div>
          <p style="font-weight: bold; margin-bottom: 5px;">用户名：${this.show_name}</p>
          <p style="color: #666; margin-bottom: 5px;">喜欢的主队：${this.show_uft}</p>
          <p style="color: #666; margin-bottom: 5px;">个性签名：${this.show_signature || '未填写'}</p>
        </div>
      </div>
      <div style="margin-top: 10px;">
        <p style="color: #666;">关注数：${this.show_follownum}</p>
        <p style="color: #666;">粉丝数：${this.show_followednum}</p>
        <p style="color: #666;">点赞数：${this.show_likenum}</p>
      </div>
    </div>
    `;
            if (mode == 1) {
                ElMessageBox({
                    title: '用户信息',
                    message: userInfoHTML,
                    showCancelButton: true,
                    confirmButtonText: confirmText,
                    cancelButtonText: '取消',
                    dangerouslyUseHTMLString: true // 允许使用 HTML 字符串
                }).then(() => {
                    this.follow()
                }).catch(() => {
                });
            } else {
                ElMessageBox({
                    title: '用户信息',
                    message: userInfoHTML,
                    showCancelButton: false,
                    confirmButtonText: '关闭',
                    dangerouslyUseHTMLString: true // 允许使用 HTML 字符串
                }).then(() => {

                }).catch(() => {
                });
            }
            return
        },
        gotoPost(post_id) {
            this.$router.push({
                path: '/detail',
                query: { clickedPostID: post_id }
            });
            this.setData();
            this.GetPostDetail(post_id);
            return;
        },
        getLength(array) {
            return array.length;
        },
    }
}
</script>

<template style="height:100%">
    <div class="common-layout">
        <div style="height: 8vh;"><my-nav /></div>
        <div class="building">
            <!--举报弹出框-->
            <el-dialog v-model="this.dialogFormVisible" title="举报详情">
                <el-form :model="form">
                    <el-form-item label="请填写举报理由" :label-width="formLabelWidth">
                        <el-input v-model="report_descriptions" autocomplete="off" />
                    </el-form-item>
                </el-form>
                <template #footer>
                    <span class="dialog-footer">
                        <el-button @click="dialogFormVisible = false">
                            放弃举报
                        </el-button>
                        <el-button type="primary" @click="dialogFormVisible = false; reportPost()">
                            确认举报
                        </el-button>
                    </span>
                </template>
            </el-dialog>
            <el-container class="post-container">
                <el-header>
                    <!--帖子抬头-->
                    <el-page-header style="margin-bottom: 1rem;" @back="goBack" :icon="ArrowLeft" />
                    <div class="header-content">
                        <div class="header-title">
                            <span>{{ title }}</span><br>
                        </div>
                        <div class="header-buttons">
                            <span style="position: relative;margin-top: 1.5vh;display: flex;align-items: center;"
                                class="header-info">
                                <el-icon size="1rem" style="margin-right:0.5vw;margin-top: 0.2vh;position: relative;">
                                    <avatar />
                                </el-icon>
                                <span style="cursor: pointer;color:#2A3E63"
                                    @click="showUserInfo(this.author_id, 1)">{{ uName }}</span>
                                发布于{{ date }}
                            </span>
                            <div style="width:40vw;justify-content: right;display: flex;">
                                <el-button class="header-respond-btn" @click="collectPost()">
                                    <span v-if="isCollected == false"><el-icon size="1rem"
                                            style="margin-right: 0.2vw;position: relative;top:0.3vh">
                                            <Star />
                                        </el-icon>收藏</span>
                                    <span v-if="isCollected == true"><el-icon size="1rem"
                                            style="margin-right: 0.2vw;position: relative;top:0.3vh">
                                            <StarFilled />
                                        </el-icon>已收藏</span>
                                </el-button>
                                <el-button :class="isApproved == false ? 'header-respond-btn' : 'header-respond-btn-active'"
                                    id="approveBtn" @click="approvePost()">
                                    <img style="width: 1vw;position: relative;top:0.3vh;margin-right: 0.5vw;"
                                        src="../assets/img/approve.png">
                                    <span style="position: relative;top:0.3vh">点赞 {{ approvalNum }}</span>
                                </el-button>
                                <el-button class="header-respond-btn" @click="this.dialogFormVisible = true">
                                    <span v-if="isReported == false"><el-icon size="1rem"
                                            style="margin-right: 0.2vw;position: relative;top:0.4vh">
                                            <Warning />
                                        </el-icon>举报</span>
                                    <span v-if="isReported == true"><el-icon size="1rem"
                                            style="margin-right: 0.2vw;position: relative;top:0.4vh">
                                            <WarningFilled />
                                        </el-icon>已举报</span>
                                </el-button>
                            </div>
                        </div>
                    </div>
                </el-header>
                <div style="height:2vh;background-color: white;"></div>
                <el-container style="border: none;">
                    <el-aside>
                        <div class="author-container">
                            <div style="cursor: pointer; height: 29vh;" @click="showUserInfo(this.author_id, 1)">
                                <img class="rooter-img" :src='avatar'>
                                <div class="rooter-name" style="width:100%">
                                    <p class="rooter-name-typography">{{ uName }}</p>
                                </div>
                            </div>
                            <el-button class="aside-follow-btn" style="width:6vw;" @click="follow()">
                                <span v-if="isFollowed == false"><el-icon size="1rem"
                                        style="margin-right: 0.2vw;position: relative;top:0.4vh">
                                        <CircleCheck />
                                    </el-icon>关注</span>
                                <span v-if="isFollowed == true"><el-icon size="1rem"
                                        style="margin-right: 0.2vw;position: relative;top:0.4vh">
                                        <CircleCheckFilled />
                                    </el-icon>已关注</span>
                            </el-button>
                        </div>
                    </el-aside>
                    <el-main>
                        <!-- 主帖 -->
                        <div style="min-height: 22vh;white-space:pre-wrap;">
                            {{ uText }}
                            <!-- <div style="display: flex;flex-direction: row;"> -->
                            <div style="display: flex;flex-direction: column;">
                                <div v-for="item in pic">
                                    <!-- <img v-if="getLength(this.pic)==3" style="width:12vw;height:12vw;margin-top: 5vh;margin-left: 2vw;" :src='item'>
                                <img v-if="getLength(this.pic)==2" style="width:17vw;height:17vw;margin-top: 5vh;margin-left: 6vw;" :src='item'>
                                <img v-if="getLength(this.pic)==1" style="width:21vw;height:auto;margin-top: 5vh;margin-left: 15vw;" :src='item'> -->
                                    <img style="width:21vw;height:auto;margin-top: 5vh;margin-left: 15vw;" :src='item'>
                                </div>
                            </div>
                        </div>
                        <!-- 评论 -->
                        <div class="judger-post" style="display: flex;flex-direction: column;">
                            <div v-for="(jName, index) in jNames">
                                <el-divider style="color: black;height:2vh"></el-divider>
                                <el-container style="height:7vh">
                                    <img style="width: 3vw;height:3vw;border-radius: 50%;" :src='jAvatar[index]'>
                                    <span style="color: #2A3E63;cursor: pointer;margin-left: 1.5vw;margin-top: 2vh;"
                                        @click="showUserInfo(this.jId[index], 0)">{{ jName }} ：</span>
                                </el-container>
                                <el-container style="margin-left: 7vw;">{{ jTexts[index] }}</el-container>
                                <el-container style="margin-top:3vh;font-size: 0.85rem;margin-left: 1vw;">{{ jDates[index]
                                }}</el-container>
                            </div>
                        </div>
                        <div style="height: 13vw;"></div>
                    </el-main>
                </el-container>
            </el-container>
            <!--右侧热门帖子-->
            <el-container class="show-hot-posts">
                <el-divider style="height:0.5vh;color: blue;weight:4px;"></el-divider>
                <span style="position: relative;left:5vw;color: #404A57;font-weight:bold;margin-bottom: 2vh;">热门帖子</span>
                <el-container v-for="(hotPost, index) in allPost" @click="gotoPost(this.allPost[index].post_id)">
                    <el-container class="single-hot-post">
                        <el-container style="justify-content: left;align-items:baseline;">
                            <span style="font-size: 1.1rem;margin-left: 0.5vw;height:1vh">{{ hotPost.title }}</span>
                        </el-container>
                        <el-container
                            style="margin-top:0px;font-size: 0.9rem;margin-left: 0.5vw;">{{ hotPost.contains }}</el-container>
                        <el-container>
                            <span style="height:1vh;font-family: KaiTi;font-size: 1rem;">
                                <span style="color:rgb(41, 93, 151);font-size: 0.8rem;">{{ hotPost.author_name }}</span>
                                发布于{{ hotPost.publishtime }}
                            </span>
                        </el-container>
                    </el-container>
                </el-container>
            </el-container>
            <!--评论输入框-->
            <div class="input-respond-container">
                <div style="margin: 18px 0">
                    <el-input v-model="userJudge" :rows="5" type="textarea" placeholder="和大家分享你的看法~" />
                    <el-button class="input-respond-btn" @click="PostJudge()">发布评论</el-button>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.building {
    /* background-color: #e3e3e3; */
    min-height: 92vh;
}

/*帖子的整体容器*/
.post-container {
    width: 70vw;
    min-height: 80vh;
    background: #f1f1f0;
    position: absolute;
    left: 40%;
    transform: translateX(-40%);
    /* 水平居中偏移修正 */
    top: 12vh;
    /* 距离页头 4vh */
    border-radius: 10px;
}

/*帖子头部*/
.el-header {
    height: 15vh;
}

.header-content {
    display: flex;
    flex-direction: column;
    height: 15vh;
    width: 68vw;
}

.header-title {
    font-family: SimHei;
    font-size: 1.5rem;
    font-weight: 200;
    position: relative;
    left: 7vw;
}

.header-author {
    height: 7vh;
    background-color: aquamarine;
}

.header-buttons {
    display: flex;
    justify-content: flex-end;
    height: 4vh;
}

.header-respond-btn {
    background-color: aliceblue;
    position: relative;
    margin-right: 0.5vw;
    display: flex;
    align-items: center;
}

.header-respond-btn-active {
    background-color: rgb(255, 201, 248);
    text-align: center;
    position: relative;
    margin-right: 0.5vw;
}

.header-info {
    font-family: SimHei;
    font-size: 1rem;
    font-weight: lighter;
    color: #9c9c9c;
}

/*左侧楼主信息栏*/
.el-aside {
    width: 15vw;
    background-color: white;
}

/*楼主信息 头像+昵称*/
.author-container {
    background-color: #f1f0f0;
    width: 14vw;
    height: 41vh;
    position: fixed;
    display: flex;
    align-items: center;
    flex-direction: column;
}

.rooter-img {
    position: relative;
    width: 6vw;
    height: 6vw;
    top: 6vh;
    align-items: center;
    /* 垂直居中对齐 */
    justify-content: center;
    /* 水平居中对齐 */
}

.rooter-name {
    position: relative;
    width: 6vw;
    height: 6vw;
    display: flex;
    align-items: center;
    /* 垂直居中对齐 */
    justify-content: center;
    /* 水平居中对齐 */
    top: 4vh;
}

.rooter-name-typography {
    font-family: KaiTi;
    font-size: 1.25rem;
    color: #2A3E63;
}

.aside-follow-btn {
    text-align: center;
    background-color: aliceblue;
    display: flex;
    align-items: center;
}

/*主信息板块*/
.el-main {
    text-align: left;
    padding-left: 3vw;
    padding-right: 3vw;
}

.bottom-detail {
    width: 100%;
    height: 10vw;
    position: relative;
    top: 3vw;
    bottom: 0;
    right: 0vw;
}

/*发布评论*/
.input-respond-container {
    background-color: rgb(251, 249, 246);
    position: fixed;
    bottom: 0;
    width: 70vw;
    height: 13vw;
    left: 11.5vw;
}

.input-respond-btn {
    text-align: center;
    background-color: #6fc8e3;
    position: relative;
    margin-top: 1vw;
    right: -60vw;
}

/*左上角返回箭头*/
.el-page-header {
    position: relative;
    top: 1.2vw;
}

.el-button--text {
    margin-right: 15px;
}

.el-select {
    width: 300px;
}

.el-input {
    width: 300px;
}

.dialog-footer button:first-child {
    margin-right: 10px;
}

/*展示热门帖子*/
.show-hot-posts {
    height: 85vh;
    width: 15vw;
    position: fixed;
    right: 1vw;
    top: 10vh;
    display: flex;
    flex-direction: column;
}

.single-hot-post {
    background-color: white;
    height: 10vh;
    margin-top: 1vh;
    display: flex;
    flex-direction: column;
    border-radius: 10px;
    cursor: pointer;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.single-hot-post:hover {
    box-shadow: 0 8px 12px rgba(0, 0, 0, 0.2);
    /* 鼠标悬浮时的阴影效果 */
}</style>
