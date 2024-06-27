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
    data() {
        return {
            AllUsers: [],
            ReportedUsers: [],
            searchkeyAll: "",
            searchkeyBanned: "",
            suspectedUser: [],
        };
    },
    mounted() {
        this.getAllUsers();
        this.getReportedUsers();
        this.getReportedPost();
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
        async getAllUsers() {
            console.log("start get all users")
            let response
            try {
                response = await axios.get('api/report/getUsrInfo');
            } catch (err) {
                console.error(err);
                if (err.response.data.result == 'fail') {
                    ElMessage.error(err.response.data.msg)
                } else {
                    ElMessage.error("未知错误")
                }
                return
            }
            //console.log("AllUsers - JSON.stringify(response) = "+JSON.stringify(response, null, 2))
            if (response.data.ok == 'no') {
                ElMessage.error("获取用户列表失败");
            } else if (response.data.ok == 'yes') {
                // 遍历传来的数据并进行转换
                response.data.value.forEach(item => {
                    const convertedItem = {
                        user_id: item.user_id,
                        userName: item.userName,
                        userPoint: item.userPoint,
                        regDate: item.regDate,
                        followedNumber: item.followedNumber || 0,
                        isBanned: item.isbanned
                    };
                    // 将转换后的数据添加到 AllUsers 数组中
                    this.AllUsers.push(convertedItem);
                    // 对 AllUsers 数组按照 user_id 进行排序
                    this.AllUsers.sort((a, b) => a.user_id - b.user_id);
                });
            }
            return
        },
        async getReportedUsers() {
            console.log("start get reported users")
            let response
            try {
                response = await axios.get('/api/Admin/GetBannedUser');
            } catch (err) {
                console.error(err);
                if (err.response.data.result == 'fail') {
                    ElMessage.error(err.response.data.msg)
                } else {
                    ElMessage.error("未知错误")
                }
                return
            }
            //console.log("ReportedUsers - JSON.stringify(response) = "+JSON.stringify(response, null, 2))
            response.data.forEach(item => {
                const convertedItem = {
                    user_id: item.user_id,
                    userName: item.username,
                    userPoint: item.userpoint,
                    regDate: this.analyse_date(item.regdate),
                    followedNumber: item.followednumber
                };
                // 将转换后的数据添加到 AllUsers 数组中
                this.ReportedUsers.push(convertedItem);
                // 对 AllUsers 数组按照 user_id 进行排序
                this.ReportedUsers.sort((a, b) => a.user_id - b.user_id);
            });
            return
        },
        async killUser(index) {
            let response
            try {
                response = await axios.post('api/report/banUsr', {
                    user_id: this.AllUsers[index].user_id,
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
                ElMessage.success("封禁用户成功");
                location.reload();
            }
            else {
                ElMessage.error("未找到相关用户信息");
            }
            return
        },
        async liftUser(index) {
            let response
            try {
                response = await axios.post('/api/Admin/LiftUser', {
                    user_id: this.ReportedUsers[index].user_id,
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
                location.reload();
                ElMessage.success("解禁用户成功");
            }
            else {
                ElMessage.error("未找到相关用户信息");
            }
            return
        },
        async searchFromAll(searchkey) {
            console.log("search key from all : " + searchkey)
            if (searchkey != "") {
                let response
                try {
                    response = await axios.post('/api/Admin/SearchUser', {
                        searchkey: String(searchkey),
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
                console.log("Search From All - JSON.stringify(response) = " + JSON.stringify(response, null, 2))
                this.AllUsers = [];
                response.data.forEach(item => {
                    const convertedItem = {
                        user_id: item.user_id,
                        userName: item.username,
                        userPoint: item.point,
                        regDate: this.analyse_date(item.createtime),
                        followedNumber: item.followednum || 0,
                        isBanned: 0
                    };
                    // 将转换后的数据添加到 AllUsers 数组中
                    this.AllUsers.push(convertedItem);
                    // 对 AllUsers 数组按照 user_id 进行排序
                    this.AllUsers.sort((a, b) => a.user_id - b.user_id);
                });
            } else {
                this.AllUsers = [];
                this.getAllUsers();
            }
            return
        },
        async searchFromBanned(searchkey) {
            console.log("search key from banned : " + searchkey)
            if (searchkey != "") {
                let response
                try {
                    response = await axios.post('/api/Admin/SearchBannedUser', {
                        searchkey: String(searchkey),
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
                console.log("Search From Banned - JSON.stringify(response) = " + JSON.stringify(response, null, 2))
                this.ReportedUsers = [];
                response.data.forEach(item => {
                    const convertedItem = {
                        user_id: item.user_id,
                        userName: item.username,
                        userPoint: item.point,
                        regDate: this.analyse_date(item.createtime),
                        followedNumber: item.followednum || 0
                    };
                    // 将转换后的数据添加到 AllUsers 数组中
                    this.ReportedUsers.push(convertedItem);
                    // 对 AllUsers 数组按照 user_id 进行排序
                    this.ReportedUsers.sort((a, b) => a.user_id - b.user_id);
                });
            } else {
                this.ReportedUsers = [];
                this.getReportedUsers();
            }
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
                    if (this.suspectedUser.includes(item.publisherName)) {

                    } else {
                        this.suspectedUser.push(item.publisherName)
                    }

                });
            }
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
                    style="overflow-y: auto;background-color:white;margin-top: 2vh;margin-left: 0.7vw;border-radius: 15px 15px 0 0;">
                    <el-table :data="AllUsers" border height="280" style="width: 100%;border-radius: 10px;">
                        <el-table-column :label="`所有用户名单`" align="center">
                            <el-table-column align="center" prop="user_id" label="用户Id" width="100" />
                            <el-table-column prop="userName" label="用户昵称" width="150" />
                            <el-table-column align="center" prop="regDate" label="注册时间" width="150" />
                            <el-table-column align="center" prop="userPoint" label="积分" width="100" />
                            <el-table-column align="center" prop="followedNumber" label="粉丝数" width="100" />
                            <el-table-column fixed="right" label="操作">
                                <template #header>
                                    <el-input placeholder="输入昵称查找用户" clearable v-model="searchkeyAll">
                                        <template #append><el-icon @click="searchFromAll(this.searchkeyAll)"
                                                style="cursor: pointer;">
                                                <search />
                                            </el-icon></template>
                                    </el-input>
                                    <!-- <el-container>操作</el-container> -->
                                </template>
                                <template #default="scope">
                                    <el-button v-if="this.AllUsers[scope.$index].isBanned == 0" link type="primary"
                                        size="small" @click="killUser(scope.$index)">封禁用户</el-button>
                                    <el-button v-if="this.AllUsers[scope.$index].isBanned == 1" disabled link type="primary"
                                        size="small">已封禁用户</el-button>
                                </template>
                            </el-table-column>
                        </el-table-column>
                    </el-table>
                    <el-table :data="ReportedUsers" border height="280"
                        style="width: 100%;border-radius: 10px;margin-top: 3vh;">
                        <el-table-column :label="`被封禁用户名单`" align="center">
                            <el-table-column align="center" prop="user_id" label="用户Id" width="100" />
                            <el-table-column prop="userName" label="用户昵称" width="150" />
                            <el-table-column align="center" prop="regDate" label="注册时间" width="150" />
                            <el-table-column align="center" prop="userPoint" label="积分" width="100" />
                            <el-table-column align="center" prop="followedNumber" label="粉丝数" width="100" />
                            <el-table-column fixed="right" label="操作">
                                <template #header>
                                    <el-input placeholder="输入昵称查找用户" clearable v-model="searchkeyBanned">
                                        <template #append><el-icon @click="searchFromBanned(this.searchkeyBanned)"
                                                style="cursor: pointer;">
                                                <search />
                                            </el-icon></template>
                                    </el-input>
                                </template>
                                <template #default="scope">
                                    <el-button link type="primary" size="small"
                                        @click="liftUser(scope.$index)">解禁用户</el-button>
                                </template>
                            </el-table-column>
                        </el-table-column>
                    </el-table>
                    <el-container style="margin-top: 3vh;margin-left: 1vw;">
                        以下用户疑似有违规行为，请管理员核实：
                        <el-container v-for="items in suspectedUser" class="suspected-name">{{ items }}</el-container>
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

.suspected-name {
    font-family: KaiTi;
    font-size: 1.1rem;
    color: #2A3E63;
}
</style>