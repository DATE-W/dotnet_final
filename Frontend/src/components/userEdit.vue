<template>
    <el-card class="card" shadow="hover" @click="uploadAvatar">
        <div class="cardInfo">
            <div>头像</div>
            <div>
                <el-avatar :src="avatarUrl" class="avatar"></el-avatar>
                <input ref="fileInput" type="file" style="display: none" @change="handleFileChange" accept="image/*" />
            </div>
        </div>
    </el-card>
    <el-card class="card" shadow="hover" style="margin-top: 3vh;" @click="openDialog('userName')">
        <div class="cardInfo">
            <div>昵称</div>
            <div>{{ userName }}</div>
        </div>
    </el-card>
    <el-card class="card" shadow="hover" style="margin-top: 3vh;" @click="openDialog('personalSign')">
        <div class="cardInfo">
            <div>个性签名</div>
            <div>{{ personalSign }}</div>
        </div>
    </el-card>
    <el-card class="card" shadow="none" style="margin-top: 3vh;">
        <div class="cardInfo">
            <div>账号</div>
            <div>{{ account }}</div>
        </div>
    </el-card>
    <div class="submit-button">
        <el-button type="primary" @click="showSubmitDialog" class="large-button">提交</el-button>
    </div>
    <el-dialog v-model="dialogVisible" :title="dialogTitle">
        <el-input v-model="tempInput"></el-input>
        <template #footer>
            <span class="dialog-footer">
                <el-button @click="cancelDialog">取消</el-button>
                <el-button type="primary" @click="confirmDialog">
                    确定
                </el-button>
            </span>
        </template>
    </el-dialog>
    <!-- 按钮放置在右下角 -->
</template>

<script>
import { ElMessage, ElMessageBox } from 'element-plus';
import axios from 'axios';

export default {
    mounted() {
        this.JudgeAccount();
    },
    data() {
        return {
            avatarUrl: '', // 头像url
            avatarFile: [],//头像的文件
            userName: "", // 用户名
            account: "", // 账号
            personalSign: '', //个性签名
            dialogVisible: false, // 修改对话框是否可见
            dialogTitle: '',
            dialogType: '',
            tempInput: '',
        };
    },
    methods: {
        async JudgeAccount() {
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/User/profile', {}, { headers })
            } catch (err) {
                ElMessage({
                    message: '未知错误',
                    grouping: false,
                    type: 'error',
                })
                return
            }
            console.log(response);
            //有账号
            if (response.data.ok == 'yes') {
                this.avatarUrl = response.data.value.avatar;
                this.userName = response.data.value.username;
                this.account = response.data.value.account;
                this.personalSign = response.data.value.signature;
            }
            else {
                ElMessage({
                    message: '登录过期，请重新登录！',
                    grouping: false,
                    type: 'error',
                })
                this.$router.push('/signin');
            }
        },
        uploadAvatar() {
            this.$refs.fileInput.click(); // 触发选择文件的对话框
        },
        handleFileChange(event) {
            const file = event.target.files[0]; // 获取选中的文件
            this.avatarFile.push(file);
            if (file) {
                // 通过 FileReader 读取选中的文件，获取其 base64 编码
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.avatarUrl = e.target.result; // 将 base64 编码设置为头像的 URL
                };
                reader.readAsDataURL(file);
            }
        },
        openDialog(buttonType) {
            if (buttonType == "userName") {
                this.dialogTitle = "修改用户昵称";
                this.dialogType = "userName";
            }
            else if (buttonType == "personalSign") {
                this.dialogTitle = "修改个性签名";
                this.dialogType = "personalSign";
            }
            this.dialogVisible = true;
        },
        confirmDialog() {
            if (this.tempInput == "") {
                ElMessage.error('输入不能为空哦~');
                return;
            }
            if (this.dialogType == "userName") {
                this.userName = this.tempInput;
            }
            else if (this.dialogType == "personalSign") {
                this.personalSign = this.tempInput;
            }
            this.dialogTitle = "";
            this.dialogType = "";
            this.tempInput = "";
            this.dialogVisible = false;
        },
        cancelDialog() {
            this.tempInput = "";
            this.dialogVisible = false;
        },
        showSubmitDialog() {
            ElMessageBox.confirm('确定要提交修改吗？', '确认', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning',
            }).then(() => {
                // 用户点击了“确定”，你可以在这里执行提交操作
                // 例如，你可以调用一个方法来向后端服务器提交数据
                this.submitData();
            }).catch(() => {
                // 用户点击了“取消”，不执行任何操作
            });
        },
        async submitPic() {
            let response
            const formData = new FormData();
            // Assuming this.avatarFiles is an array of selected files
            for (const file of this.avatarFile) {
                formData.append('files', file);
            };
            try {
                response = await axios.post('/api/Picture/SaveFile', formData, { 'Content-Type': 'multipart/form-data' });
            } catch (err) {
                console.log(err);
                ElMessage({
                    message: '未知错误',
                    grouping: false,
                    type: 'error',
                })
                return
            };
            this.avatarUrl = response.data.value[response.data.value.length - 1];
            console.log(response);
        },
        async submitData() {
            const temp = this.avatarUrl;
            await this.submitPic(); // 提交图片，获得返回的图片url
            const serverip = 'http://110.40.206.206/';
            const userName = this.userName;
            const sign = this.personalSign;

            if (this.avatarUrl == undefined) {
                this.avatarUrl = temp;
            }
            else {
                this.avatarUrl = serverip + this.avatarUrl;
            }
            const avatar = this.avatarUrl;
            //这里加后端交互代码，然后刷新当前页面
            // 延迟刷新页面
            const token = localStorage.getItem('token');
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/User/modifyprofile',
                    {
                        username: String(userName),
                        signature: String(sign),
                        avatar: String(avatar),
                    },
                    { headers })
            } catch (err) {
                console.log(err);
                ElMessage({
                    message: '未知错误',
                    grouping: false,
                    type: 'error',
                })
                return
            }
            console.log(response);
            //有账号
            if (response.data.ok == 'yes') {
                ElMessage({
                    message: '修改成功！',
                    grouping: false,
                    type: 'success',
                })
            }
            else {
                ElMessage({
                    message: '登录过期，请重新登录！',
                    grouping: false,
                    type: 'error',
                })
                // this.$router.push('/signin');
            }
        }
    },
}
</script>

<style scoped>
/* 图片风格 */
.avatar {
    width: 50px;
    height: 50px;
    border-radius: 48px;
    margin-top: 0.7vh;
    margin-left: 1vw;
    object-fit: cover;
}

.inputbox {
    display: flex;
    width: 35vw;
    height: 4vh;
    font-size: 1rem;
}

.card {
    font-size: 24px;
}

.cardInfo {
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 100%;
}

/* 按钮样式 */
.submit-button {
    display: flex;
    justify-content: center;
    align-items: center;
    margin-top: 15vh;
}

/* 新增按钮样式 */
.large-button {
    font-size: 24px;
    /* 增大字体大小 */
    padding: 12px 20px;
    /* 增加上下左右的内边距 */
    width: 120px;
    /* 增加按钮宽度 */
    height: 50px;
}
</style>