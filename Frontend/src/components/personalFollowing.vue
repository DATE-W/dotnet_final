<template>
    <div class="follow-list">
        <div class="follow-container">
            <el-card class="follow-card" v-for="(user, index) in followList" :key="index">
                <div class="user-info" @click="showUserInfo(index)">
                    <img :src="user.avatar" alt="User Avatar" class="avatar" />
                    <div class="user-details">
                        <p class="username">{{ user.userName }}</p>
                        <p class="bio">{{ truncateText(user.signature, 20) }}</p>
                    </div>
                </div>
                <div class="follow-button">
                    <el-button type="primary" size="small" @click="followOnclick(index)"
                        v-if="user.isfollowed == 1">取消关注</el-button>
                    <el-button type="primary" size="small" @click="followOnclick(index)" v-else>关注</el-button>
                </div>
            </el-card>
        </div>
    </div>
</template>
  
<script>
import axios from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';
export default {
    data() {
        return {
            followList: [],  // 用户数据...
        };
    },
    mounted() {
        this.getfollowList();
    },
    methods: {
        async getfollowList() {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/User/followList', {}, { headers });
            } catch (err) {
                console.log(err);
            }
            console.log(response);
            this.followList = response.data;
        },
        truncateText(text, limit) {
            if (text.length <= limit) {
                return text;
            } else {
                const truncatedText = text.slice(0, limit);
                return truncatedText + '...';
            }
        },
        async showUserInfo(index) {
            const confirmText = this.followList[index].isfollowed ? '取消关注' : '关注';
            // 构建用户信息的 HTML 字符串
            const userInfoHTML = `
    <div style="padding: 10px;">
      <div style="display: flex; align-items: center;">
        <div style="margin-right: 10px;">
          <img src="${this.followList[index].avatar}" alt="User Avatar" style="width: 50px; height: 50px; border-radius: 50%;" />
        </div>
        <div>
          <p style="font-weight: bold; margin-bottom: 5px;">用户名：${this.followList[index].userName}</p>
          <p style="color: #666; margin-bottom: 5px;">喜欢的主队：${this.followList[index].uft}</p>
          <p style="color: #666; margin-bottom: 5px;">个人称号：${await this.getUserTitle(this.followList[index].userpoint)}</p>
          <p style="color: #666; margin-bottom: 5px;">个性签名：${this.followList[index].signature || '未填写'}</p>
        </div>
      </div>
      <div style="margin-top: 10px;">
        <p style="color: #666;">关注数：${this.followList[index].follownum}</p>
        <p style="color: #666;">粉丝数：${this.followList[index].fansnum}</p>
        <p style="color: #666;">点赞数：${this.followList[index].likenum}</p>
      </div>
    </div>
    `;
            ElMessageBox({
                message: userInfoHTML,
                showCancelButton: true,
                confirmButtonText: confirmText,
                cancelButtonText: '取消',
                dangerouslyUseHTMLString: true // 允许使用 HTML 字符串
            }).then(() => {
                this.followOnclick(index)
            }).catch(() => {
            });
        },
        async getUserTitle(mypoint) {
            if (mypoint >= 0 && mypoint <= 9) return '平平无奇';
            if (mypoint >= 10 && mypoint <= 49) return '普通用户';
            if (mypoint >= 50 && mypoint <= 99) return '一贴成名';
            if (mypoint >= 100 && mypoint <= 499) return '球场金童';
            if (mypoint >= 500 && mypoint <= 999) return '明日之星';
            if (mypoint >= 1000) return '名人堂';
        },
        // 点击 关注/取消关注
        async followOnclick(index) {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                const userID = this.followList[index].user_id;
                this.followList[index].isfollowed = this.followList[index].isfollowed === 1 ? 0 : 1;
                response = await axios.post('/api/Forum/FollowbyUserid', { follow_id: userID }, { headers })
            } catch (err) {
                console.log(err);
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
                    return
                }
                return
            }
            console.log(response);
            if (this.followList[index].isfollowed) {
                this.$emit('follow-event', 'follow');
            }
            else {
                this.$emit('follow-event', 'unfollow');
            }

        }
    }
};
</script>
  
<style scoped>
.follow-list {
    display: flex;
    flex-wrap: wrap;
    gap: 20px;
}

.follow-container {
    width: 100%;
    max-height: 720px;
    /* 设置最大高度 */
    overflow-y: auto;
    /* 添加垂直滚动条 */
    display: flex;
    flex-wrap: wrap;
    gap: 20px;
}

.follow-container::-webkit-scrollbar {
    width: 5px;
    /* 设置滚动条宽度 */
}

.follow-container::-webkit-scrollbar-thumb {
    background-color: #888;
    /* 设置滚动条颜色 */
    border-radius: 5px;
    /* 设置滚动条圆角 */
}

.follow-card {
    width: calc(33.33% - 20px);
    /* Three cards per row with some gap */
}

.user-info {
    display: flex;
    align-items: center;
}

.avatar {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    margin-right: 10px;
}

.user-details {
    flex: 1;
}

.username {
    font-weight: bold;
}

.bio {
    font-size: 12px;
    color: #666;
}

.follow-button {
    text-align: center;
    margin-top: 10px;
}
</style>
