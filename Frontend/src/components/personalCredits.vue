<!-- 我的积分 v2.0 -->
<template>
    <div class="overflow-container">
        <div class="main-container">
            <div class="left-container">
                <div class="user-title">{{ usertitle }}</div>
                <div class="user-point">
                    {{ userpoint }}
                    <span class="user-point-else">积分</span>
                </div>
                <div class="rectangle-container">
                    <div v-for="(height, index) in rectangleHeights" :key="index" class="rectangle" :style="{
                        height: height + 'px',
                        marginTop: (rectangleHeights[maxHeightIndex] - height) + 'px',
                        backgroundColor: (index === userIndex) ? '#0BE4F7' : '#E2EFEE'
                    }">
                        <div class="rectangle-point">{{ point[index] }}</div>
                        <div class="rectangle-title">{{ title[index] }}</div>
                    </div>
                </div>
                <div class="point-detail">
                    <span class="detail-title">积分明细:</span>
                    <ul class="infinite-list" style="overflow: auto">
                        <li v-for="(detail, index) in pointDetails" :key="index" class="infinite-list-item">
                            {{ getDetailText(detail) }}
                        </li>
                    </ul>
                </div>
            </div>
            <div class="user-equity">
                <div class="equity-title">积分摘要</div>
                <div>
                    <el-collapse v-model="activeNames" @change="handleChange">
                        <el-collapse-item name="1">
                            <template #title>
                                <span style="display: inline-block; min-width: 320px;">积分规则</span>
                            </template>
                            <div>
                                每个用户的账号中拥有积分，<br>
                                根据积分的多少将账号的等级分为：<br>
                                "平平无奇"、"普通用户"、"一贴成名"、<br>
                                "球场金童"、"明日之星"以及"名人堂"，<br>
                                会在用户的名片上展示对应的称号，<br>
                                其它用户也可以查看。
                            </div>
                        </el-collapse-item>
                        <el-collapse-item title="获取方式" name="2">
                            <div>
                                积分的获取方式如下：<br>
                                发帖成功+10分;<br>
                                每日签到+5分;<br>
                                评论成功+3分;<br>
                                收藏+1分;<br>
                                点赞+1分
                            </div>
                        </el-collapse-item>
                    </el-collapse>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.overflow-container {
    overflow-y: auto;
    max-height: 625px;
}

.overflow-container::-webkit-scrollbar {
    width: 0;
}

.left-container {
    flex: 1;
}

.user-title {
    margin-top: 10px;
    font-family: YouYuan;
    font-weight: bold;
    font-size: 24px;
    margin-bottom: 10px;
}

.user-point {
    font-size: 20px;
}

.user-point-else {
    font-family: STZhongsong;
    font-size: 15px;
    margin-left: 5px;
}

.main-container {
    margin-left: 20px;
    display: flex;
    align-items: flex-start;
}

.rectangle-container {
    margin-left: 30px;
    display: flex;
    position: relative;
}

.rectangle {
    width: 100px;
    margin-right: 20px;
    border-radius: 8px;
    position: relative;
}

.rectangle-point {
    position: absolute;
    top: -20px;
    left: 0;
    width: 100%;
    text-align: center;
    font-size: 15px;
    color: #000;
}

.rectangle-title {
    position: absolute;
    bottom: -25px;
    left: 0;
    width: 100%;
    text-align: center;
    font-size: 15px;
    color: #000;
}

.user-equity {
    margin-top: 10px;
    margin-left: 50px;
    max-width: 340px;
}

.equity-title {
    font-size: 18px;
    margin-bottom: 20px;
}

.point-detail {
    margin-top: 100px;
}

.detail-title {
    font-size: 18px;
}

.infinite-list {
    height: 300px;
    padding: 0;
    margin: 0;
    list-style: none;
}

.infinite-list .infinite-list-item {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 50px;
    background: var(--el-color-primary-light-9);
    margin: 10px;
    color: var(--el-color-primary);
}

.infinite-list .infinite-list-item+.list-item {
    margin-top: 10px;
}
</style>

<script lang="ts">
import { ref } from 'vue';
import axios from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';
export default {
    data() {
        return {
            usertitle: '',  //向后端请求获得用户的称号
            userpoint: 0,  //向后端请求获得用户的积分数
            rectangleHeights: [30, 50, 70, 100, 140, 200],  //矩形的长度
            title: ["平平无奇", "普通用户", "一贴成名", "球场金童", "明日之星", "名人堂"],  //称号名
            point: [0, 10, 50, 100, 500, 1000],  //各称号的最低积分
            userIndex: -1,   //判断用户是哪个称号范围
            pointDetails: [],  // 存储积分明细操作的数组
        };
    },
    mounted() {
        this.getPoint();
        this.getDetails();
    },
    computed: {
        maxHeightIndex() {
            // 找到最高矩形的高度在数组中的索引
            return this.rectangleHeights.indexOf(Math.max(...this.rectangleHeights));
        },
        findUserIndex() {
            for (let i = this.point.length - 1; i >= 0; i--) {
                if (this.userpoint >= this.point[i]) {
                    return i;
                }
            }
            return -1;
        },
    },
    methods: {
        getDetailText(detail) {
            switch (detail) {
                case 'like':
                    return '点赞帖子 积分+1';
                case 'comment':
                    return '评论帖子 积分+3';
                case 'collect':
                    return '收藏帖子 积分+1';
                case 'publish':
                    return '发布帖子 积分+10';
                case 'checkin':
                    return '签到成功 积分+5';
                default:
                    return '';
            }
        },
        getUserTitle(mypoint) {
            if (mypoint >= 0 && mypoint <= 9) return '平平无奇';
            if (mypoint >= 10 && mypoint <= 49) return '普通用户';
            if (mypoint >= 50 && mypoint <= 99) return '一贴成名';
            if (mypoint >= 100 && mypoint <= 499) return '球场金童';
            if (mypoint >= 500 && mypoint <= 999) return '明日之星';
            if (mypoint >= 1000) return '名人堂';
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
                this.userpoint = response.data.userpoint;
                this.userIndex = this.findUserIndex;  //进入页面即显示用户的称号
                this.usertitle = this.getUserTitle(this.userpoint);
            } catch (err) {
                if (err.response.data.result == 'fail') {
                    ElMessage({
                        message: err.response.data.msg,
                        grouping: false,
                        type: 'error',
                    })
                } else {
                    ElMessage({
                        message: '服务器错误，获取积分失败',
                        grouping: false,
                        type: 'error',
                    })
                    return
                }
                return
            }
        },
        async getDetails() {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/User/userPointAction', {}, { headers });
                if (response.data && Array.isArray(response.data)) {
                    response.data.forEach((detail) => {
                        this.pointDetails.push(detail);
                    });
                };
                console.log('积分详情', response);
            } catch (err) {
                console.error('Error fetching details:', err);
            }
        },
    },
    setup() {
        const activeNames = ref(['0']);
        const handleChange = (val: string[]) => {
            console.log(val);
        };
        return { activeNames, handleChange };
    }//折叠面板
};
</script>