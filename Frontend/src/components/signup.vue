<script>
import carousel from './signinCarousel.vue';
import { ElMessage } from 'element-plus';
import axios from 'axios';
import { sha256 } from 'js-sha256'
export default {
  data() {
    return {
      account: '',
      userName: '',
      password: '',
      confirmpassword: '',
      securityQ: '',
      securityAns: ''
    };
  },
  components: {
    'my-carousel': carousel
  },
  methods: {
    validateInput() {
      // 使用正则表达式检查输入是否符合要求
      const regex = /^[a-zA-Z0-9]+$/;
      if (!regex.test(this.account)) {
        // 如果输入不符合要求，将其修正为仅包含数字和字母的内容
        this.account = this.account.replace(/[^a-zA-Z0-9]+/g, '');
      }
    },
    async register() {
      if (!this.account) {
        ElMessage({
          message: '请输入用户名',
          grouping: false,
          type: 'error',
        })
        return
      }
      if (!this.password) {
        ElMessage({
          message: '请输入密码',
          grouping: false,
          type: 'error',
        })
        return
      }
      if (!this.confirmpassword) {
        ElMessage({
          message: '请再次输入密码',
          grouping: false,
          type: 'error',
        })
        return
      }
      if (this.password != this.confirmpassword) {
        ElMessage({
          message: '两次密码不一致',
          grouping: false,
          type: 'error',
        })
        return
      }
      let response
      try {
        response = await axios.post('/api/Register/normalRegister', {
          account: String(this.account),
          userName: String(this.userName),
          // password: String(this.password),
          password: String(await sha256(this.password)),
          userSecQue: String(this.securityQ),
          // userSecAns: String(this.securityAns),
          userSecAns: String(await sha256(this.securityAns)),
        })
      } catch (err) {
        ElMessage({
          message: '未知错误',
          grouping: false,
          type: 'error',
        })
        return
      }
      if (response.data.ok == "no") {
        ElMessage({
          message: '账号已存在！',
          grouping: false,
          type: 'error',
        })
        // 延迟刷新页面
        setTimeout(() => {
          window.location.reload(); // 刷新当前页面
        }, 2000); // 2000毫秒后刷新，你可以根据需要调整延迟时间
        return
      }
      else {
        ElMessage({
          message: '注册成功，请重新登录',
          grouping: false,
          type: 'success',
        })
      }
      this.$router.push('/signin')
    },
    redirectToLogin() {
      // 跳转到登录页面的逻辑
      this.$router.push('/signin');
    },
  }
};
</script>

<template>
  <div class="common-layout">
    <el-container>
      <el-aside>
        <my-carousel />
      </el-aside>
      <el-main>
        <el-page-header @back="$router.push('/')"></el-page-header>
        <img src="../assets/img/recover_logo.png" class="imgR">
        <!-- 注册 -->
        <div style="bottom:3vh">
          <el-space direction="vertical" style="text-align:center">
            <h1 class="labelH1 excenter">Register to create your account</h1>
            <el-text tag="i" class="excenter" style="font-size: 0.9rem;">Set password and account for creation</el-text>
          </el-space>
        </div>
        <!-- 输入内容 -->
        <div class="maininputbox">
          <form @submit.prevent="register">
            <!-- 输入用户名 -->
            <div class="subBox">
              <label for="account_name" class="inputText" left=10.3vw;>用户名：</label>
              <el-input type="text" id="account_name" v-model="userName" pattern="[a-zA-Z0-9]+" required maxlength="10"
                class="inputBox" placeholder="用户名长度不超过10个字符" />
            </div>
            <!-- 输入账号 -->
            <div class="subBox">
              <label for="account" class="inputText" left=10.3vw;>账号：</label>
              <el-input type="text" id="account" v-model="account" required maxlength="10" class="inputBox"
                placeholder="账号只能由数字和字母组成，且长度不超过10个字符" @input="validateInput" />

            </div>
            <!-- 输入密码 -->
            <div class="subBox">
              <label for="password" class="inputText" left=10.3vw;>密码：</label>
              <el-input type="password" id="password" v-model="password" pattern="[a-zA-Z0-9]+" required maxlength="15"
                class="inputBox" show-password placeholder="密码只能由数字和字母组成，且长度不超过15个字符" @input="validateInput" />
            </div>
            <!-- 再次输入密码 -->
            <div class="subBox">
              <label for="confirmpassword" class="inputText">确认密码：</label>
              <el-input type="password" id="confirmpassword" v-model="confirmpassword" pattern="[a-zA-Z0-9]+" required
                maxlength="15" class="inputBox" show-password placeholder="请再次输入密码" />
            </div>
            <!-- 密保问题 -->
            <div class="subBox">
              <label for="securityQ" class="inputText">密保问题：</label>
              <el-input type="text" id="securityQ" v-model="securityQ" pattern="[\u4e00-\u9fa5\d\s\p{P}]+" required
                maxlength="10" class="inputBox  " placeholder="密保问题只能包含中文、字母、数字和标点符号，且长度不超过10个字符" />
            </div>
            <!-- 密保答案 -->
            <div class="subBox">
              <label for="securityAns" class="inputText">密保答案：</label>
              <el-input type="text" id="securityAns" v-model="securityAns" pattern="[\u4e00-\u9fa5\d\s\p{P}]+" required
                maxlength="10" class="inputBox" placeholder="密保答案只能包含中文、字母、数字和标点符号，且长度不超过10个字符" />
            </div>
            <div>
              <el-text tag="i" class="labeltext">Please remember your password and security！</el-text>
            </div>

          </form>
        </div>
        <!-- 注册按钮 -->
        <div>
          <el-text @click="redirectToLogin" tag="i" class="havePos haveText">已经有账号了？</el-text>
          <button type="submit" class="signupBtn" @click="register">
            <text class="btnText">注册</text>
          </button>
        </div>
      </el-main>
    </el-container>
  </div>
</template>

<style scoped>
.el-aside {
  position: absolute;
  background: linear-gradient(180deg, #77B0FE 0%, rgba(119, 176, 254, 0.10) 100%);
  top: 0;
  left: 0;
  width: 50%;
  height: 100%;
}

.el-main {
  position: absolute;
  top: 0;
  right: 0;
  width: 50%;
  height: 100%;
}

/*用来装入每一个子标题+输入框的box*/
.subBox {
  position: relative;
  left: 5%;
  width: 25vw;
  height: 4.3vw;
  flex-shrink: 0;
}

/*每一个子标题的位置及样式*/
.inputText {
  left: 6vw;
  position: relative;
  color: var(--gray-3, #828282);
  font-family: Nunito Sans;
  font-size: 0.85rem;
  font-style: normal;
  font-weight: 700;
  line-height: normal;
}

/*每一个输入框的样式及内置提示*/
.inputBox {
  position: relative;
  left: 5vw;
  display: flex;
  width: 33vw;
  padding: 0.5vh 0.6vw;
  align-items: center;
  gap: 0;
  border-radius: 0.31rem;
  border: 0;
}

/* 注册按钮＋已经有账号了 */
.signupBtn {
  position: relative;
  left: 1.5vw;
  width: 30vw;
  height: 2.6vw;
  top: 6.7vh;
  flex-shrink: 0;
  border-radius: 0.31rem;
  background: #007DFA;
  border: 0;
}

.btnText {
  color: #FFF;
  font-family: Poppins;
  font-size: 1rem;
  font-style: normal;
  font-weight: 700;
  line-height: normal;
}

.havePos {
  position: relative;
  left: 67%;
  width: 0.5vw;
  height: 2.5vw;
  top: 11vh;
}

.haveText {
  cursor: pointer;
  color: #494F7A;
  font-family: Nunito Sans;
  font-size: 1rem;
  font-style: normal;
  font-weight: 400;
  line-height: normal;
}

/*右侧左上角logo*/
.imgR {
  top: 3vh;
  left: 4vw;
  width: 4vw;
  height: 7vh;
  position: relative;
}

/*注册标题 */
.labelH1 {
  color: #525252;
  font-family: Nunito Sans;
  font-size: 1.8rem;
  font-style: normal;
  font-weight: 700;
  line-height: normal;
}

/*右侧标题位置设置 */
.excenter {
  text-align: center;
  line-height: 1;
  position: relative;
  top: 3vh;
  left: 9.5vw;
}

/*右侧输出框控制 */
.maininputbox {
  position: relative;
  top: 5vh;

}

.labeltext {
  color: #7F265B;
  font-size: 0.6rem;
  position: relative;
  left: 23vw;
  top: -1vh;
  bottom: 1vh;
}
</style>