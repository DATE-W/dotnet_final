<script>
import axios from 'axios';
import { ElMessage } from 'element-plus';
import dashboard from './AdminDashBoard.vue';
import headerview from './AdminNav.vue';
export default {
    components:{
        dashboard:dashboard,
        headerview:headerview,
    },
    data() {
        return {
           announcement:[],
        };
    },
    mounted(){
        this.getData();
    },
    methods:{
        analyse_date(date){
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
      async getData()
      {
        console.log("start get past announcement")
            let response
            try {
                response = await axios.get('/api/Admin/GetAllNotice');
            } catch (err) {
                console.error(err);
                if (err.response.data.result == 'fail') {
                    ElMessage.error(err.response.data.msg)
                } else {
                    ElMessage.error("未知错误")
                }
                return
            }
            console.log("PastAnnouncement - JSON.stringify(response) = "+JSON.stringify(response, null, 2))
            // 遍历传来的数据并进行转换
            response.data.forEach(item => {
                const convertedItem = {
                    notice_id:item.notice_id,
                    text: item.text,
                    publishdatetime: this.analyse_date(item.publishdatetime),
                    //receiver:response.data.receiver
                };
            //将转换后的数据添加到 reportedPost 数组中
            this.announcement.push(convertedItem);
            this.announcement.sort((a, b) => a.notice_id - b.notice_id);
            });
            return
      }
    }
}
</script>

<template>
    <div id="building">
        <el-container class="rooter-box">
        <el-header class="hide-header">
            <headerview/>
        </el-header>
        <el-container>
            <el-aside width="20vw" class="hide-aside">
            <dashboard/>
            </el-aside>
            <el-main style="background-color:white;margin-top: 2vh;margin-left: 0.7vw;border-radius: 15px 15px 0 0;">
                <el-container class="past-anc-title">
                    <el-divider style="height:7vh;"><el-container class="title">往期公告列表</el-container></el-divider>
                </el-container>
                <el-container style="overflow-y:auto;display: flex;flex-direction: column;height:78vh;">
                    <el-container v-for="(pastAnc) in announcement">
                        <el-container class="single-anc">
                            <el-container class="single-notice-id">
                                <el-container class="notice-id-line"/>
                                <el-container class="notice-id">{{ pastAnc.notice_id }}</el-container>
                            </el-container>
                            <el-container class="single-notice-main">
                                <el-container class="text"  style="padding-right:1vw;"><el-icon style="margin-top:0.5vh;margin-right:2vw;"><ChatLineSquare /></el-icon>{{ pastAnc.text }}</el-container>
                                <el-container class="publishdatetime"><el-icon style="margin-top:0.5vh;"><Avatar /></el-icon>发布于<span style="color: rgb(77, 172, 207);">{{ pastAnc.publishdatetime }}</span></el-container>
                            </el-container>
                        </el-container>
                    </el-container>
                </el-container>
            </el-main>
        </el-container>
        </el-container>
    </div>
</template>

<style scoped>
@media (max-width: 768px) { /* 设置适当的最大宽度值 */
  .hide-aside {
    display: none;
  }
  .hide-header {
    display: none;
  }
}

#building{
    background-color:#eee;
    left:0px;
    top:0px;
    width:100vw;			
    height:100vh;		
    position: fixed;
}

.rooter-box{
    position: fixed;
    width:80vw;
    height:100vh;
    left: 10vw;
}
/*展示往期公告*/
.past-anc-title{
    height: 7vh;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;

}
.title{
    color:#404A57;
    font-weight: bold;
    font-size:large;
}
.single-anc{
    height:13vh;
    margin-top: 4vh;
}
.single-notice-id{
    width: 20%;

}
.single-notice-main{
    background-color: #e3e3e3;
    display: flex;
    flex-direction: column;
    border-radius: 15px;
    margin:0.5vh 0.5vw 0.5vh 0;
    width: 80%;
}
.notice-id{
    position: relative;
    margin:2vh 4vh 2vh 2vh;
    border-radius: 15px;
    border: 1px solid #727171;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
}
.text{
    padding-top: 1vh;
    padding-left: 1vw;
    height:10vh
}
.publishdatetime{
    justify-content: right;
    padding-right:2vw;
    padding-bottom: 1vh;
}
</style>