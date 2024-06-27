<template>
    <div>
        <!-- 当前球队展示 -->
        <div style="display: flex; flex-direction: column; justify-content: center; align-items: center; height: 140px;">
            <div v-if="selectedteam.chinesename !== ''">
                <div style="font-size: 24px; margin-bottom: 10px;display: flex;  justify-content: center;">当前主队</div>
                <div style="display: flex;  justify-content: center;">
                    <img :src="selectedteam.logo" alt="Selected team" class="team-circle" />
                </div>
                <div style="display: flex;  justify-content: center;">
                    <div style="font-size: 20px;">{{ selectedteam.chinesename }}</div>
                </div>
            </div>
            <div v-else style="font-size: 20px;">暂时没有选择主队</div>
        </div>
        <div class="default-button" v-if="selectedteam.chinesename !== ''">
            <el-button type="primary" @click="recover" class="default-buttonsize">取消选择主队</el-button>
        </div>
        <el-divider></el-divider>

        <!-- 球队选项 -->
        <el-row class="frame-options">
            <el-col :span="8" v-for="(team, index) in teamList.slice(1)" :key="index">
                <div class="frame-option">
                    <div class="team-item" @click="showteamPreview(team)">
                        <div class="team-circle" :style="`background-image: url(${team.logo})`"
                            :class="{ 'selected-frame': selectedteam.chinesename === team.chinesename }"></div>
                        <div class="team-name">{{ team.chinesename }}</div>
                    </div>
                </div>
            </el-col>
        </el-row>

    </div>
</template>
  
<script>
import { ElMessage, ElMessageBox } from 'element-plus';
import axios from 'axios';
export default {
    mounted() {
        this.getTeam();
    },
    data() {
        return {
            teamList: [],
            selectedteam: { chinesename: '', logo: '/' }, // 初始化选中的球队
            previewImageUrl: '' // 用于保存预览图片的 URL
        };
    },
    methods: {
        async getTeam() {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/User/getallteam', {}, { headers })
            } catch (err) {
                console.log(err)
                ElMessage({
                    message: '未知错误',
                    grouping: false,
                    type: 'error',
                })
                return
            }
            this.teamList = response.data;
            console.log(this.teamList)
            if (this.teamList[0].chinesename == '无主队') {
                this.selectedteam.chinesename = '';
                this.selectedteam.logo = '/';
            }
            else {
                this.selectedteam = this.teamList[0];
            }
            return;
        },
        showteamPreview(team) {
            this.previewImageUrl = team.logo;

            ElMessageBox({
                title: '切换主队',
                message: `
                    <div>
                        <p>是否切换成球队 ${team.chinesename}？</p>
                        <img src="${team.logo}" alt="team Preview" style="max-width: 100%;">
                    </div>
                `,
                showCancelButton: true,
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                dangerouslyUseHTMLString: true // 允许使用 HTML 字符串
            }).then(() => {
                this.selectteam(team);
            }).catch(() => {
                this.previewImageUrl = '';
            });
            return
        },
        async selectteam(team) {
            this.selectedteam = team;
            // 执行选中球队相关操作
            this.previewImageUrl = '';
            const teamname = this.selectedteam.chinesename;
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/User/modifyteam', { teamname }, { headers })
            } catch (err) {
                console.log(err)
                ElMessage({
                    message: '未知错误',
                    grouping: false,
                    type: 'error',
                })
                return
            }
            console.log(response);
            return
        },
        async recover() {
            ElMessageBox({
                title: '切换主队',
                message: `
                    <div>
                        <p>是否取消选择主队？</p>
                    </div>
                `,
                showCancelButton: true,
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning',
                dangerouslyUseHTMLString: true // 允许使用 HTML 字符串
            }).then(async () => {
                const token = localStorage.getItem('token');
                let response
                try {
                    const headers = {
                        Authorization: `Bearer ${token}`,
                    };
                    response = await axios.post('/api/User/modifyteam', { teamname: '' }, { headers })
                } catch (err) {
                    console.log(err)
                    ElMessage({
                        message: '未知错误',
                        grouping: false,
                        type: 'error',
                    })
                    return
                }
                console.log(response);
                if (response.data.value = '主队删除成功') {
                    this.selectedteam = { chinesename: '', logo: '/' };
                }
            }).catch(() => {
                this.previewImageUrl = '';
            });
            return
        }
    }
};
</script>
  
<style scoped>
.el-col {
    justify-content: center;
    display: flex;
}

/* 添加所需的样式规则 */
.frame-options {
    margin: 20px;
    text-align: center;
    overflow-y: auto;
    /* 添加垂直滚动条 */
    max-height: 65vh;
    /* 设置最大高度，超过部分会出现滚动条 */
}

.frame-option {
    width: 100px;
    height: 150px;
    margin: 5px;
    cursor: pointer;
}

.selected-frame {
    outline: 5px solid #1890ff;
    /* 使用 outline 代替 border */
}

.team-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    cursor: pointer;
}

.team-circle {
    width: 100px;
    height: 100px;
    background-size: cover;
    border-radius: 50%;
}

.team-name {
    margin-top: 5px;
    font-size: 16px;
}

.default-button {
    display: flex;
    position: relative;
    justify-content: flex-end;
}

.default-buttonsize {
    font-size: 20px;
    height: 50px;
}
</style>
