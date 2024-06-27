<!-- 签到 v2.0 -->
<template>
  <!-- 添加外层的容器，并使用Flexbox布局 -->
  <div class="overflow-container">
    <div class="checkin-container">
      <el-config-provider :locale="zhCn">
        <el-calendar ref="calendar">
          <template #date-cell="{ data }">
            <p>
              {{ formatDate(data.day) }}
              {{ isSelectedDate(data.day) ? '✔️' : '' }}
            </p>
          </template>
        </el-calendar>
      </el-config-provider>
      <el-button class="checkin-button" type="primary" @click="CheckIn">点此签到</el-button>
    </div>
  </div>
</template>
  
<script lang="ts">
import { ref } from 'vue';
import type { CalendarDateType, CalendarInstance } from 'element-plus';
import { ElMessage, ElMessageBox } from 'element-plus';
import zhCn from 'element-plus/es/locale/lang/zh-cn';
import axios from 'axios';
export default {
  data() {
    return {
      calendar: ref<CalendarInstance>(),
      selectedDates: [],
      zhCn
    };
  },
  mounted() {
    this.JudgeCheckTime();
  },
  methods: {
    isSelectedDate(date: string) {
      return this.selectedDates.includes(date);
    },
    formatDate(date: string) {
      const [fullDate, _] = date.split('T'); // 剥离日期部分
      const [year, month, day] = fullDate.split('-');
      return `${parseInt(month)}-${parseInt(day)}`;
    },
    BackendDate(date: string) {
      const [fullDate, _] = date.split('T'); // 剥离日期部分
      const [year, month, day] = fullDate.split('-');
      const formattedMonth = month.padStart(2, '0'); // 月份以两位数形式显示
      const formattedDay = day.padStart(2, '0');     // 日期以两位数形式显示
      return `${parseInt(year)}-${formattedMonth}-${formattedDay}`;
    },
    async JudgeCheckTime() {
      const token = localStorage.getItem('token');
      let response;
      try {
        const headers = {
          Authorization: `Bearer ${token}`,
        };
        response = await axios.post('/api/User/UserCheckTime', {}, { headers });
        if (Array.isArray(response.data)) {
          response.data.forEach((date) => {
            const formattedDate = this.BackendDate(date);
            if (!this.selectedDates.includes(formattedDate)) {
              this.selectedDates.push(formattedDate);
            }
          });
        }
      } catch (err) {
        if (err.response.data.result == 'fail') {
          ElMessage({
            message: err.response.data.msg,
            grouping: false,
            type: 'error',
          })
        } else {
          ElMessage({
            message: '服务器错误，获取过往签到日期失败',
            grouping: false,
            type: 'error',
          })
          return
        }
        return
      }
    },
    async CheckIn() {
      const currentDate = new Date().toISOString().slice(0, 10); //获取当前日期
      //console.log(currentDate);
      const token = localStorage.getItem('token');
      let response
      try {
        const headers = {
          Authorization: `Bearer ${token}`,
        };
        //判断selectedDates数组里是否有当前日期
        if (this.selectedDates.includes(currentDate)) {
          //有则说明今日已经签到过 不和后端交互
          ElMessageBox.confirm(
            '签到失败，今日已经签到过！',
            {
              confirmButtonText: '确认',
              cancelButtonText: '取消',
              type: 'warning',
            }
          );
        } else {
          //没有则交互 添加日期
          response = await axios.post('/api/User/UserCheckin', {}, { headers });
          if (response.data.ok == 'yes') {
            this.selectedDates.push(currentDate);
            ElMessage({
              message: '签到成功，积分+5！',
              type: 'success',
            });
          }
        }
      } catch (err) {
        if (err.response.data.result == 'fail') {
          ElMessage({
            message: err.response.data.msg,
            grouping: false,
            type: 'error',
          });
        } else {
          ElMessage({
            message: '服务器错误，签到失败',
            grouping: false,
            type: 'error',
          });
        }
      }
    },
  }
}
</script>
  
<style scoped>
.overflow-container {
  overflow-y: auto;
  max-height: 625px;
}

.overflow-container::-webkit-scrollbar {
  width: 0;
}

.checkin-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
}

.checkin-button {
  font-size: large;
  width: 200px;
  height: 60px;
}
</style>
  