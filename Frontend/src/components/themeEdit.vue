<template>
    <div>
        <!-- 当前主题展示 -->
        <div class="currentTheme" :style="{ backgroundImage: `url(${selectedTheme.image3})` }">
        </div>
    </div>
    <el-divider></el-divider>
    <div>
        <!-- 主题选项 -->
        <el-row class="frame-options">
            <el-col :span="8" v-for="(theme, index) in themeList" :key="index">
                <div class="frame-option" v-if="index < themeList.length - 1">
                    <div class="theme-item" @click="showThemePreview(theme)">
                        <div class="theme-circle" :style="`background-image: url(${theme.image1})`"
                            :class="{ 'selected-frame': selectedTheme.id === theme.id }"></div>
                        <div class="theme-name">{{ theme.name }}</div>
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
        this.getalltheme();
    },
    data() {
        return {
            themeList: [],
            selectedTheme: {
                id: 0,
                name: '',
                image1: '',
                image2: '',
                image3: '',
                image4: ''
            }, // 初始化选中的主题
            previewImageUrl: './src/assets/img/carousel1.png' // 用于保存预览图片的 URL
        };
    },
    methods: {
        async getalltheme() {
            const token = localStorage.getItem('token');
            if (token == null) {
                return;
            }
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/User/getalltheme', {}, { headers })
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
            this.themeList = response.data;
            this.selectedTheme = response.data[response.data.length - 1];
            console.log(this.selectedTheme)
        },
        showThemePreview(theme) {
            this.previewImageUrl = theme.image3;
            ElMessageBox({
                title: '切换主题',
                message: `
                    <div>
                        <p>是否切换到主题 ${theme.name}？</p>
                        <img src=${theme.image3} alt="Theme Preview" style="max-width: 100%;">
                    </div>
                `,
                showCancelButton: true,
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                dangerouslyUseHTMLString: true // 允许使用 HTML 字符串
            }).then(() => {
                this.selectTheme(theme);
            }).catch(() => {
                this.previewImageUrl = '';
            });
        },
        async selectTheme(theme) {
            this.selectedTheme = theme;
            // 执行选中主题相关操作
            const token = localStorage.getItem('token');
            if (token == null) {
                return;
            }
            let response
            try {
                const headers = {
                    Authorization: `Bearer ${token}`,
                };
                response = await axios.post('/api/User/modifytheme', { theme_id: theme.id }, { headers })
            } catch (err) {
                console.log(err);
                ElMessage({
                    message: '未知错误',
                    grouping: false,
                    type: 'error',
                })
                return
            }

            this.previewImageUrl = '';
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
    max-height: 40vh;
    /* 设置最大高度，超过部分会出现滚动条 */
}

.frame-options::-webkit-scrollbar {
    width: 5px;
    /* 设置滚动条宽度 */
}

.frame-options::-webkit-scrollbar-thumb {
    background-color: #888;
    /* 设置滚动条颜色 */
    border-radius: 5px;
    /* 设置滚动条圆角 */
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

.theme-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    cursor: pointer;
}

.theme-circle {
    width: 100px;
    height: 100px;
    background-size: cover;
    border-radius: 10px;
    /* 调整这里的值来设置圆角大小 */
}


.theme-name {
    margin-top: 5px;
    font-size: 12px;
}

.currentTheme {
    align-items: center;
    background-size: cover;
    background-repeat: no-repeat;
    background-position: center;
    height: 40vh;
    padding: 20px;
    /* 添加内边距以使文字内容不紧贴边界 */
}
</style>
