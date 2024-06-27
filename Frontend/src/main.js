import './assets/css/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import ElementPlus from 'element-plus';
import 'element-plus/theme-chalk/index.css';
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import { createI18n } from 'vue-i18n'; // 导入 vue-i18n

const app = createApp(App)

app.use(router)
app.use(ElementPlus)
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(key, component)
}
import en from './i18n/en.json';
import zhCN from './i18n/zh-CN.json';
const i18n = createI18n({
    locale: 'zhCN', // 默认语言
    messages: {
        en,
        zhCN
    }
});

app.use(i18n); // 使用 i18n

app.mount('#app')
