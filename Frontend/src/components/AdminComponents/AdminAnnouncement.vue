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
           announcement:'',
        };
    },
    methods:{
        clearAnnouncementInput()
        {
            this.announcement='';
            return
        },
        async subbmitAnnouncement()
        {
            console.log("subbmitAnnouncement "+ this.announcement)
            let response
            try {
                response = await axios.post('api/notice/createNotice',  {
                    admin_id:0,
                    text:String(this.announcement),
                });
                console.log(JSON.stringify(response.data))
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
            console.log(response);
            if (response.data.ok == 'yes') {
                ElMessage.success("发布成功");
            }
            else {
               ElMessage.error("发布失败");
            }
            this.announcement='';
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
            <el-main style="overflow: hidden;background-color:white;margin-top: 2vh;margin-left: 0.7vw;border-radius: 15px 15px 0 0;">
                <el-container class="main-container">
                    <el-container class="announcement-tittle">发布公告</el-container>
                    <el-container>
                        <el-input  
                            class="announcement-input-box"
                            v-model="announcement"
                            :rows="8"
                            type="textarea"
                            placeholder="请输入公告内容"
                        />
                    </el-container>
                    <el-container>
                        <el-button  style="margin-left:12vw;" class="announcement-btn" @click=" clearAnnouncementInput">
                            <span class="announcement-btn-text">清空</span>
                        </el-button>
                        <el-button class="announcement-btn" @click="subbmitAnnouncement">
                            <span class="announcement-btn-text">发布</span>
                        </el-button>
                    </el-container>
                </el-container>
                <el-container class="rules">
                    <el-container style="margin-bottom:2vh;padding-left:15vw;font-weight:bold;font-size:1.3rem;">
                        管理员公告守则
                    </el-container>
                    <el-container style="margin-bottom:2vh;">
                        第一条&nbsp;&nbsp;一切以事实为依据。
                    </el-container>
                    <el-container style="margin-bottom:2vh;">
                        第二条&nbsp;&nbsp;行文完整，体现具体内容。
                    </el-container>
                    <el-container style="margin-bottom:2vh;">
                        第三条&nbsp;&nbsp;相关数据，引文和文字表述准确，恰当。
                    </el-container>
                    <el-container style="margin-bottom:2vh;">
                        第四条&nbsp;&nbsp;汉字，标点符号，计量单位等用法符合公文处理条例的有关规定。
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
/*发布公告栏*/
.announcement-tittle{
    font-family: 'Courier New', Courier, monospace;
    color: #404A57;
    font-weight: bold;
    font-weight: 2rem;
    font-size: 1.5rem;
    text-align: center;
    margin-top: 2vh;
    position: relative;
    left: 24vw;
}
.announcement-input-box{
    position: relative;
    width:90%;
    left: 2.5vw;
}
.announcement-btn{
    border-radius: 3rem;
    width:15vw;
    height:5vh;
    text-align: center;
    background-color: rgb(124, 119, 254);
}
.announcement-btn-text{
    font-family:'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;
    color: #ffffff;
    font-weight: 2rem;
    font-size: 1rem;
    position: relative;
}
.main-container{
    height: 70%;
    display: flex;
    flex-direction: column;
    justify-content: center;
}
.rules{
    border:1px dashed;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: left;
    padding-left:8vw;
    padding-top: 2vh;
}
</style>