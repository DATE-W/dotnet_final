<template>
    <el-container class="maincontainer">
        <el-header class="lrborder" style="height: 5vh;">
            <el-page-header @back="goBack">
                <template #content>
                    <span> 资料编辑 </span>
                </template>
            </el-page-header>
        </el-header>
        <el-divider style="margin: 0;" />
        <el-main class="lrborder" style="padding: 0px; height: 95vh;">
            <el-tabs v-model="activeName" type="border-card" tab-position="left" style="height: 100%; border: 0;"
                class="demo-tabs">
                <el-tab-pane name="useredit">
                    <template #label>
                        <span class="custom-tabs-label">
                            <el-icon>
                                <UserFilled />
                            </el-icon>
                            <span>个人资料</span>
                        </span>
                    </template>
                    <userEdit />
                </el-tab-pane>
                <el-tab-pane name="themeedit">
                    <template #label>
                        <span class="custom-tabs-label">
                            <el-icon>
                                <Monitor />
                            </el-icon>
                            <span>个性装扮</span>
                        </span>
                    </template>
                    <themeEdit />
                </el-tab-pane>
                <el-tab-pane name="teamedit">
                    <template #label>
                        <span class="custom-tabs-label">
                            <el-icon>
                                <Football />
                            </el-icon>
                            <span>个人主队</span>
                        </span>
                    </template>
                    <hometeamEdit />
                </el-tab-pane>
            </el-tabs>
        </el-main>
    </el-container>
</template>
  
<script>
import userEdit from './userEdit.vue';
import themeEdit from './themeEdit.vue';
import hometeamEdit from './hometeamEdit.vue';
export default {
    mounted() {
        if (this.$route.query.value) {
            this.activeName = this.$route.query.value;
        }
    },
    components: {
        'userEdit': userEdit,
        'themeEdit': themeEdit,
        'hometeamEdit': hometeamEdit,
    },
    data() {
        return {
            avatarUrl: "./src/assets/img/carousel1.png", // 头像url
            userName: "WinWin", // 用户名
            account: "1145141919810", // 账号
            dialogVisible: false, // 修改对话框是否可见
            dialogTitle: '',
            activeName: 'useredit',
        };
    },
    methods: {
        goBack() {
            this.$router.push('/personal');
        },
        openDialog(buttonType) {
            if (buttonType == "username") {
                this.dialogTitle = "修改用户昵称";
            }
            this.dialogVisible = true;
        },

    },
};
</script>
  
<style scoped>
.maincontainer {
    height: 100vh;
    /* 上右下左 */
    padding: 0 20vw 0 20vw;
}

/* 页面左右的边框 */
.lrborder {
    border-left: 1px solid #EAEAEA;
    border-right: 1px solid #EAEAEA;
}

.el-tabs--right .el-tabs__content,
.el-tabs--left .el-tabs__content {
    height: 100%;
}

.demo-tabs .custom-tabs-label .el-icon {
    vertical-align: middle;
}

.demo-tabs .custom-tabs-label span {
    vertical-align: middle;
    margin-left: 4px;
    font-size: 16px;
}
</style>
  